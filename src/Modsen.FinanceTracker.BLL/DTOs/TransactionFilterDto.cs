using System.Linq.Expressions;
using Modsen.FinanceTracker.Domain.Entities;

namespace Modsen.FinanceTracker.BLL.DTOs;

public sealed record TransactionFilterDto
{
    public string? SearchTerm { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    
    public Expression<Func<Transaction, bool>> ToExpression()
    {
        return t => 
            (string.IsNullOrWhiteSpace(SearchTerm) || t.Description.Contains(SearchTerm)) &&
            (!From.HasValue || t.Date >= From.Value) &&
            (!To.HasValue || t.Date <= To.Value);
    }
}