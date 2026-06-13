using Modsen.FinanceTracker.BLL.Interfaces;
using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.Domain.Enums;

namespace Modsen.FinanceTracker.BLL.Factories;

public sealed class TransactionFactory : ITransactionFactory
{
    public Transaction CreateTransaction(
        TransactionType type, decimal amount, string description, Guid categoryId)
    {
        var id = Guid.NewGuid();
        var date = DateTime.Now;

        return type switch
        {
            TransactionType.Income => new IncomeTransaction(id, amount, description, date, categoryId),
            TransactionType.Expense => new ExpenseTransaction(id, amount, description, date, categoryId),

            _ => throw new ArgumentException("Invalid transaction type", nameof(type))
        };
    }
}