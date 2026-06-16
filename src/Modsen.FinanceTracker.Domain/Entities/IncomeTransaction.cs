using Modsen.FinanceTracker.Domain.ValueObjects;

namespace Modsen.FinanceTracker.Domain.Entities;

public sealed class IncomeTransaction(
    Guid id,
    Money amount,
    TransactionDescription description,
    TransactionDate date,
    Category category) : Transaction(id, amount, description, date, category) 
{
}
