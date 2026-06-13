namespace Modsen.FinanceTracker.Domain.Events;

public sealed class CategoryLimitExceededEventArgs
{
    public string CategoryName { get; set; } = string.Empty;
    public decimal ExcessAmount { get; set; }
}
