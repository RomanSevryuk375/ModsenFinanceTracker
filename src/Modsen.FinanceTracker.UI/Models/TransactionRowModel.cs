namespace Modsen.FinanceTracker.UI.Models;

public sealed record TransactionRowModel(
    string Id,
    string Date,
    string Description,
    string CategoryName,
    string FormattedAmount);