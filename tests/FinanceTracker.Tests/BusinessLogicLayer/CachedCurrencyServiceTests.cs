using Microsoft.Extensions.Caching.Memory;
using Modsen.FinanceTracker.BLL.Decorators;
using Modsen.FinanceTracker.BLL.Interfaces;

namespace Modsen.FinanceTracker.Tests.BLL.Decorators;

public sealed class CachedCurrencyServiceTests
{
    [Fact]
    public async Task GetExchangeRateAsyncFirstCallShouldFetchFromInnerServiceAndSaveToCacheAsync()
    {
        // Arrange
        ICurrencyService decorated = Substitute.For<ICurrencyService>();
        IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());

        var sut = new CachedCurrencyService(decorated, cache);

        decorated.GetExchangeRateAsync("USD", "EUR", Arg.Any<CancellationToken>())
            .Returns(Result.Success(1.08m));

        // Act
        Result<decimal> result = await sut.GetExchangeRateAsync("USD", "EUR", CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(1.08m);
        cache.TryGetValue("key_USD_EUR", out decimal rate).Should().BeTrue();
        rate.Should().Be(1.08m);

        await decorated.Received(1).GetExchangeRateAsync("USD", "EUR", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetExchangeRateAsyncSecondCallShouldReturnFromCacheWithoutCallingInnerServiceAsync()
    {
        // Arrange
        ICurrencyService decorated = Substitute.For<ICurrencyService>();
        IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());
        cache.Set("key_USD_EUR", 1.08m);

        var sut = new CachedCurrencyService(decorated, cache);

        // Act
        Result<decimal> result = await sut.GetExchangeRateAsync("USD", "EUR", CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(1.08m);

        await decorated.DidNotReceive().GetExchangeRateAsync(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }
}
