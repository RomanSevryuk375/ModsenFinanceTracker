using Modsen.FinanceTracker.BLL.Interfaces;
using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.Domain.Enums;

namespace Modsen.FinanceTracker.BLL.Factories;

public sealed class TransactionFactory : ITransactionFactory
{
    public Transaction CreateTransaction(
        TransactionType type, decimal amount, string description, Category category)
    {
        var id = Guid.NewGuid();
        DateTime date = DateTime.Now;

        return type switch
        {
            TransactionType.Income => new IncomeTransaction(id, amount, description, date, category),
            TransactionType.Expense => new ExpenseTransaction(id, amount, description, date, category),

            _ => throw new ArgumentException("Invalid transaction type", nameof(type))
        };
    }
}