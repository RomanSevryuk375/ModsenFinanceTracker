using System.Net.Http.Json;
using Modsen.FinanceTracker.BLL.DTOs;
using Modsen.FinanceTracker.BLL.Interfaces;
using Modsen.FinanceTracker.Domain.Extensions;

namespace Modsen.FinanceTracker.Infrastructure.Services;

public sealed class CurrencyService(
    HttpClient httpClient) : ICurrencyService
{
    private const string ApiBaseUrl = "https://open.er-api.com/v6/latest/";

    public async Task<Result<decimal>> GetExchangeRateAsync(
        string fromCurrency,
        string toCurrency,
        CancellationToken cancellationToken = default)
    {
        fromCurrency = fromCurrency.ToUpper();
        toCurrency = toCurrency.ToUpper();

        if (fromCurrency == toCurrency)
        {
            return Result.Success(1m);
        }

        try
        {
            HttpResponseMessage response = await httpClient.GetAsync(
                $"{ApiBaseUrl}{fromCurrency}", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return Result.Fail<decimal>($"API responded with status code: {response.StatusCode}");
            }

            ExchangeRateResponse? data = await response.Content.ReadFromJsonAsync<ExchangeRateResponse>(
                cancellationToken: cancellationToken);

            if (data is null ||
                data.Status != "success" ||
                !data.Rates.TryGetValue(toCurrency, out decimal rate))
            {
                return Result.Fail<decimal>($"Rate for {toCurrency} not found.");
            }

            return Result.Success(rate);
        }
        catch (HttpRequestException ex)
        {
            return Result.Fail<decimal>($"Network error: {ex.Message}");
        }
        catch (Exception ex)
        {
            return Result.Fail<decimal>($"Unexpected error during currency fetch: {ex.Message}");
        }
    }
}
