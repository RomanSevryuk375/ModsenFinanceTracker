using Modsen.FinanceTracker.Domain.Extensions;
using Modsen.FinanceTracker.Domain.ValueObjects;

namespace Modsen.FinanceTracker.BLL.Factories;

public sealed class TransactionFactory : ITransactionFactory
{
    public Transaction? CreateTransaction(
        TransactionType type, Money amount, TransactionDescription description, Category category)
    {
        var id = Guid.NewGuid();
        Result<TransactionDate> date = TransactionDate.Create(DateTime.UtcNow);
        if (date.IsFailure)
        {
            return null;
        }

        return type switch
        {
            TransactionType.Income => new IncomeTransaction(id, amount, description, date.Value, category),
            TransactionType.Expense => new ExpenseTransaction(id, amount, description, date.Value, category),

            _ => throw new ArgumentException("Invalid transaction type", nameof(type))
        };
    }
}
