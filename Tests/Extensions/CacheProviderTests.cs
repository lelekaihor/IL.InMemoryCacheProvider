﻿using IL.InMemoryCacheProvider.Extensions;
using Xunit;

namespace IL.InMemoryCacheProvider.Tests.Extensions
{
    public class CacheProviderTests
    {
        private const string Key = "testkey";
        private const string Tag = "testTag";
        private const string ExpectedValue = "newValue";

        [Fact]
        public async Task GetAllKeysAsync_Returns_ExistingKeys_When_CacheContains_key()
        {
            // Arrange
            var cacheProvider = new CacheProvider.InMemoryCacheProvider();
            await cacheProvider.GetOrAddAsync(Key, () => ExpectedValue);

            // Act
            var result = await cacheProvider.GetAllKeysAsync();

            // Assert
            Assert.Contains(Key, result);
        }

        [Fact]
        public async Task DeleteAllAsync_Deletes_ExistingKeys_When_CacheContains_Objects()
        {
            // Arrange
            var cacheProvider = new CacheProvider.InMemoryCacheProvider();
            await cacheProvider.GetOrAddAsync(Key, () => ExpectedValue);
            await cacheProvider.DeleteAllAsync();

            // Act
            var result = await cacheProvider.GetAllKeysAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task Add_Tags_And_Eviction_By_Tag_For_Cache_Objects()
        {
            // Arrange
            var cacheProvider = new CacheProvider.InMemoryCacheProvider();
            await cacheProvider.GetOrAddAsync(Key,
                () => ExpectedValue,
                tags: new[] { Tag });
            
            await cacheProvider.EvictByTagAsync(Tag);

            // Act
            var result = await cacheProvider.GetAllKeysAsync();

            // Assert
            Assert.Empty(result);
        }
    }
}