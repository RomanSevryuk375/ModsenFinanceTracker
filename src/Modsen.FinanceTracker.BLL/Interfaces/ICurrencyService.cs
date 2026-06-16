using Modsen.FinanceTracker.Domain;

namespace Modsen.FinanceTracker.BLL.Interfaces;

public interface ICurrencyService
{
    public Task<Result<decimal>> GetExchangeRateAsync(
        string fromCurrency,
        string toCurrency,
        CancellationToken cancellationToken = default);
}
