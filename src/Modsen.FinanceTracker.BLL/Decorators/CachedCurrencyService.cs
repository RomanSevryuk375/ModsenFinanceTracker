using Microsoft.Extensions.Caching.Memory;
using Modsen.FinanceTracker.Domain.Extensions;

namespace Modsen.FinanceTracker.BLL.Decorators;

public sealed class CachedCurrencyService(
    ICurrencyService decorated,
    IMemoryCache distributed) : ICurrencyService
{
    public async Task<Result<decimal>> GetExchangeRateAsync(
        string fromCurrency,
        string toCurrency,
        CancellationToken cancellationToken = default)
    {
        string upperTo = toCurrency.ToUpper();
        string upperFrom = fromCurrency.ToUpper();

        if (upperFrom == upperTo)
        {
            return Result.Success(1m);
        }

        string cacheKey = $"key_{upperFrom}_{upperTo}";

        if (distributed.TryGetValue(cacheKey, out decimal rate))
        {
            return Result.Success(rate);
        }

        Result<decimal> result = await decorated.GetExchangeRateAsync(
            upperFrom, upperTo, cancellationToken);
        if (result.IsSuccess)
        {
            MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(1));

            distributed.Set(cacheKey, result.Value, cacheOptions);
        }

        return result;
    }
}
