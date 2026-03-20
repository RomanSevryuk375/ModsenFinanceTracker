namespace Modsen.FinanceTracker.UI.Models;

public record TransactionRowModel(
    string Id, 
    string Date, 
    string Description, 
    string CategoryName,
    string FormattedAmount);