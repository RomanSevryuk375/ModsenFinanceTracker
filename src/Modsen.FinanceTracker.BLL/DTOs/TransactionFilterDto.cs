namespace Modsen.FinanceTracker.BLL.DTOs;

public sealed record TransactionFilterDto
{
    public string? SearchTerm { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }

    public Func<Transaction, bool> ToFilter()
    {
        return t =>
            (string.IsNullOrWhiteSpace(SearchTerm) ||
             t.Description.Value.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)) &&
            (!From.HasValue || t.Date.Value >= From.Value) &&
            (!To.HasValue || t.Date.Value <= To.Value);
    }
}
