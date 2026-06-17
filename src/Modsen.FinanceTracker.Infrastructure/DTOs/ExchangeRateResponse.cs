using System.Text.Json.Serialization;

namespace Modsen.FinanceTracker.Infrastructure.DTOs;

public sealed record ExchangeRateResponse(
    [property: JsonPropertyName("result")] string Status,
    [property: JsonPropertyName("base_code")] string BaseCurrency,
    [property: JsonPropertyName("rates")] Dictionary<string, decimal> Rates,
    DateTime ReceivedAt);
