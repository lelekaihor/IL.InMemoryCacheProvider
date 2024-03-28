﻿using System.Collections.Concurrent;
using IL.InMemoryCacheProvider.Extensions;
using IL.InMemoryCacheProvider.Options;
using Microsoft.Extensions.Caching.Memory;

namespace IL.InMemoryCacheProvider.CacheProvider;

public sealed class InMemoryCacheProvider : ICacheProvider
{
    private static readonly ConcurrentDictionary<string, HashSet<string>> TagIndex = new();
    private readonly MemoryCache _cache;

    public InMemoryCacheProvider(MemoryCacheOptions? options = null)
    {
        _cache = new MemoryCache(options ?? new MemoryCacheOptions());
    }

    public void Add<T>(string key, T? obj, ExpirationOptions? expirationOptions = default,
        IEnumerable<string>? tags = default)
    {
        if (obj == null)
        {
            return;
        }

        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expirationOptions?.AbsoluteExpirationRelativeToNow,
            AbsoluteExpiration = expirationOptions?.AbsoluteExpiration,
            SlidingExpiration = expirationOptions?.SlidingExpiration
        };
        _cache.Set(key, obj, cacheEntryOptions);
        if (tags == default)
        {
            return;
        }

        var tagsEnumerated = tags.ToList();
        foreach (var tag in tagsEnumerated)
        {
            TagIndex.AddOrUpdate(tag,
                _ => new HashSet<string> { key },
                (_, set) =>
                {
                    set.Add(key);
                    return set;
                });
        }

        cacheEntryOptions.RegisterPostEvictionCallback((_, _, _, _) =>
        {
            foreach (var tag in tagsEnumerated)
            {
                if (TagIndex.TryGetValue(tag, out var tagKeys))
                {
                    tagKeys.Remove(key);
                    if (!tagKeys.Any())
                    {
                        TagIndex.TryRemove(tag, out _);
                    }
                }
            }
        });
    }

    public Task AddAsync<T>(string key,
        T? obj,
        ExpirationOptions? expirationOptions = default,
        IEnumerable<string>? tags = default)
    {
        Add(key, obj, expirationOptions, tags);
        return Task.CompletedTask;
    }

    public T? Get<T>(string key) => _cache.Get<T>(key);

    public Task<T?> GetAsync<T>(string key)
    {
        return Task.FromResult(Get<T>(key));
    }

    public void Delete(string key)
    {
        _cache.Remove(key);
    }

    public Task DeleteAsync(string key)
    {
        Delete(key);
        return Task.CompletedTask;
    }

    public void EvictByTag(string tag)
    {
        if (!TagIndex.TryRemove(tag, out var cacheKeys))
        {
            return;
        }

        foreach (var cacheKey in cacheKeys)
        {
            Delete(cacheKey);
        }
    }

    public Task EvictByTagAsync(string tag)
    {
        EvictByTag(tag);
        return Task.CompletedTask;
    }

    public bool HasKey(string key)
    {
        return _cache.TryGetValue(key, out _);
    }

    public Task<IEnumerable<string>> GetAllKeysAsync()
    {
        return Task.FromResult(GetAllKeys());
    }

    public IEnumerable<string> GetAllKeys()
    {
        return _cache.GetKeys<string>();
    }

    public async Task DeleteAllAsync()
    {
        foreach (var key in await GetAllKeysAsync())
        {
            await DeleteAsync(key);
        }
    }

    public void DeleteAll()
    {
        foreach (var key in GetAllKeys())
        {
            Delete(key);
        }
    }
}