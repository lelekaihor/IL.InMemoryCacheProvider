﻿using IL.InMemoryCacheProvider.CacheProvider;
using IL.InMemoryCacheProvider.Extensions;
using Moq;
using Xunit;

namespace IL.InMemoryCacheProvider.Tests.Extensions
{
    public class CacheProviderExtensionsTests
    {
        private const string Key = "testkey";
        private const string ExpectedValue = "newValue";

        [Fact]
        public async Task GetOrAddAsync_Returns_ExistingValue_When_CacheContains_key()
        {
            // Arrange
            var cacheProviderMock = new Mock<ICacheProvider>();

            cacheProviderMock.Setup(x => x.GetAsync<string>(Key)).ReturnsAsync(ExpectedValue);

            // Act
            var result = await cacheProviderMock.Object.GetOrAddAsync(
                Key,
                () =>
                {
                    throw new InvalidOperationException("Value factory should not be called.");
#pragma warning disable CS0162 // Unreachable code detected
                    return string.Empty;
#pragma warning restore CS0162 // Unreachable code detected
                },
                x => !string.IsNullOrEmpty(x));

            // Assert
            Assert.Equal(ExpectedValue, result);
        }

        [Fact]
        public async Task GetOrAddAsync_Returns_NewValue_When_CacheDoesNotContain_key()
        {
            // Arrange
            var cacheProviderMock = new Mock<ICacheProvider>();
            var valueFactoryCalled = false;

            cacheProviderMock.Setup(x => x.GetAsync<string>(Key))
                .ReturnsAsync((string)null!);

            // Act
            var result = await cacheProviderMock.Object.GetOrAddAsync(Key,
                () =>
                {
                    valueFactoryCalled = true;
                    return ExpectedValue;
                },
                x => !string.IsNullOrEmpty(x));

            // Assert
            Assert.Equal(ExpectedValue, result);
            Assert.True(valueFactoryCalled, "Value factory should be called.");
            cacheProviderMock.Verify(x => x.AddAsync(Key, ExpectedValue, null, null), Times.Once);
        }

        [Fact]
        public async Task GetOrAddAsync_AsyncFactory_Returns_ExistingValue_When_CacheContains_key()
        {
            // Arrange
            var cacheProviderMock = new Mock<ICacheProvider>();

            cacheProviderMock.Setup(x => x.GetAsync<string>(Key)).ReturnsAsync(ExpectedValue);

            // Act
            var result = await cacheProviderMock.Object.GetOrAddAsync(
                Key, () =>
                {
                    throw new InvalidOperationException("Value factory should not be called.");
#pragma warning disable CS0162 // Unreachable code detected
                    return Task.FromResult(string.Empty);
#pragma warning restore CS0162 // Unreachable code detected
                },
                x => !string.IsNullOrEmpty(x));

            // Assert
            Assert.Equal(ExpectedValue, result);
        }

        [Fact]
        public async Task GetOrAddAsync_AsyncFactory_Returns_NewValue_When_CacheDoesNotContain_key()
        {
            // Arrange
            var cacheProviderMock = new Mock<ICacheProvider>();
            var valueFactoryCalled = false;

            cacheProviderMock.Setup(x => x.GetAsync<string>(Key))
                .ReturnsAsync((string)null!);

            // Act
            var result = await cacheProviderMock.Object.GetOrAddAsync(Key, () =>
                {
                    valueFactoryCalled = true;
                    return Task.FromResult(ExpectedValue);
                },
                x => !string.IsNullOrEmpty(x));

            // Assert
            Assert.Equal(ExpectedValue, result);
            Assert.True(valueFactoryCalled, "Value factory should be called.");
            cacheProviderMock.Verify(x => x.AddAsync(Key, ExpectedValue, null, null), Times.Once);
        }

        [Fact]
        public void GetOrAdd_Returns_ExistingValue_When_CacheContains_key()
        {
            // Arrange
            var cacheProviderMock = new Mock<ICacheProvider>();
            cacheProviderMock.Setup(x => x.GetAsync<string>(Key)).ReturnsAsync(ExpectedValue);

            // Act
            var result = cacheProviderMock.Object.GetOrAdd(
                Key,
                () =>
                {
                    throw new InvalidOperationException("Value factory should not be called.");
#pragma warning disable CS0162 // Unreachable code detected
                    return string.Empty;
#pragma warning restore CS0162 // Unreachable code detected
                },
                x => !string.IsNullOrEmpty(x));

            // Assert
            Assert.Equal(ExpectedValue, result);
        }

        [Fact]
        public void GetOrAdd_Returns_NewValue_When_CacheDoesNotContain_key()
        {
            // Arrange
            var cacheProviderMock = new Mock<ICacheProvider>();
            var valueFactoryCalled = false;

            cacheProviderMock.Setup(x => x.GetAsync<string>(Key))
                .ReturnsAsync((string)null!);

            // Act
            var result = cacheProviderMock.Object.GetOrAdd(Key,
                () =>
                {
                    valueFactoryCalled = true;
                    return ExpectedValue;
                },
                x => !string.IsNullOrEmpty(x));

            // Assert
            Assert.Equal(ExpectedValue, result);
            Assert.True(valueFactoryCalled, "Value factory should be called.");
            cacheProviderMock.Verify(x => x.AddAsync(Key, ExpectedValue, null, null), Times.Once);
        }

        [Fact]
        public void GetOrAdd_AsyncFactory_Returns_ExistingValue_When_CacheContains_key()
        {
            // Arrange
            var cacheProviderMock = new Mock<ICacheProvider>();
            cacheProviderMock.Setup(x => x.GetAsync<string>(Key)).ReturnsAsync(ExpectedValue);

            // Act
            var result = cacheProviderMock.Object.GetOrAdd(
                Key,
                () =>
                {
                    throw new InvalidOperationException("Value factory should not be called.");
#pragma warning disable CS0162 // Unreachable code detected
                    return Task.FromResult(string.Empty);
#pragma warning restore CS0162 // Unreachable code detected
                },
                x => !string.IsNullOrEmpty(x));

            // Assert
            Assert.Equal(ExpectedValue, result);
        }

        [Fact]
        public void GetOrAdd_AsyncFactory_Returns_NewValue_When_CacheDoesNotContain_key()
        {
            // Arrange
            var cacheProviderMock = new Mock<ICacheProvider>();
            var valueFactoryCalled = false;

            cacheProviderMock.Setup(x => x.GetAsync<string>(Key))
                .ReturnsAsync((string)null!);

            // Act
            var result = cacheProviderMock.Object.GetOrAdd(Key, () =>
                {
                    valueFactoryCalled = true;
                    return Task.FromResult(ExpectedValue);
                },
                x => !string.IsNullOrEmpty(x));

            // Assert
            Assert.Equal(ExpectedValue, result);
            Assert.True(valueFactoryCalled, "Value factory should be called.");
            cacheProviderMock.Verify(x => x.AddAsync(Key, ExpectedValue, null, null), Times.Once);
        }

        [Fact]
        public async Task GetOrAddAsync_SlidingExpiration_Returns_ExistingValue_When_CacheContains_Key()
        {
            // Arrange
            var cacheProviderMock = new Mock<ICacheProvider>();
            var slidingExpiration = TimeSpan.FromMinutes(30);

            cacheProviderMock.Setup(x => x.GetAsync<string>(Key)).ReturnsAsync(ExpectedValue);

            // Act
            var result = await cacheProviderMock.Object.GetOrAddAsync(
                Key,
                () =>
                {
                    throw new InvalidOperationException("Value factory should not be called.");
#pragma warning disable CS0162 // Unreachable code detected
                    return string.Empty;
#pragma warning restore CS0162 // Unreachable code detected
                },
                x => !string.IsNullOrEmpty(x),
                slidingExpiration: slidingExpiration);

            // Assert
            Assert.Equal(ExpectedValue, result);
            cacheProviderMock.Verify(x => x.AddAsync(Key, ExpectedValue, null, slidingExpiration), Times.Never);
        }

        [Fact]
        public async Task GetOrAddAsync_SlidingExpiration_Returns_NewValue_When_CacheDoesNotContain_Key()
        {
            // Arrange
            var cacheProviderMock = new Mock<ICacheProvider>();
            var valueFactoryCalled = false;
            var slidingExpiration = TimeSpan.FromMinutes(30);

            cacheProviderMock.Setup(x => x.GetAsync<string>(Key))
                .ReturnsAsync((string)null!);

            // Act
            var result = await cacheProviderMock.Object.GetOrAddAsync(Key,
                () =>
                {
                    valueFactoryCalled = true;
                    return ExpectedValue;
                },
                x => !string.IsNullOrEmpty(x),
                slidingExpiration: slidingExpiration);

            // Assert
            Assert.Equal(ExpectedValue, result);
            Assert.True(valueFactoryCalled, "Value factory should be called.");
            cacheProviderMock.Verify(x => x.AddAsync(Key, ExpectedValue, null, slidingExpiration), Times.Once);
        }

        [Fact]
        public void GetOrAdd_SlidingExpiration_Returns_ExistingValue_When_CacheContains_Key()
        {
            // Arrange
            var cacheProviderMock = new Mock<ICacheProvider>();
            var slidingExpiration = TimeSpan.FromMinutes(30);

            cacheProviderMock.Setup(x => x.GetAsync<string>(Key)).ReturnsAsync(ExpectedValue);

            // Act
            var result = cacheProviderMock.Object.GetOrAdd(
                Key,
                () =>
                {
                    throw new InvalidOperationException("Value factory should not be called.");
#pragma warning disable CS0162 // Unreachable code detected
                    return string.Empty;
#pragma warning restore CS0162 // Unreachable code detected
                },
                x => !string.IsNullOrEmpty(x),
                slidingExpiration: slidingExpiration);

            // Assert
            Assert.Equal(ExpectedValue, result);
            cacheProviderMock.Verify(x => x.AddAsync(Key, ExpectedValue, null, slidingExpiration), Times.Never);
        }

        [Fact]
        public void GetOrAdd_SlidingExpiration_Returns_NewValue_When_CacheDoesNotContain_Key()
        {
            // Arrange
            var cacheProviderMock = new Mock<ICacheProvider>();
            var valueFactoryCalled = false;
            var slidingExpiration = TimeSpan.FromMinutes(30);

            cacheProviderMock.Setup(x => x.GetAsync<string>(Key))
                .ReturnsAsync((string)null!);

            // Act
            var result = cacheProviderMock.Object.GetOrAdd(Key,
                () =>
                {
                    valueFactoryCalled = true;
                    return ExpectedValue;
                },
                x => !string.IsNullOrEmpty(x),
                slidingExpiration: slidingExpiration);

            // Assert
            Assert.Equal(ExpectedValue, result);
            Assert.True(valueFactoryCalled, "Value factory should be called.");
            cacheProviderMock.Verify(x => x.AddAsync(Key, ExpectedValue, null, slidingExpiration), Times.Once);
        }
    }
}