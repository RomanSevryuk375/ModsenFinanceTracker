namespace Modsen.FinanceTracker.UI.Models;

public sealed record AnalyticsRowModel(
    string CategoryName,
    string FormattedPercent,
    string FormattedAmount,
    double RawAmount);