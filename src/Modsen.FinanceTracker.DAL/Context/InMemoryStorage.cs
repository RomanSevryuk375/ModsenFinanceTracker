using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.Domain.Enums;

namespace Modsen.FinanceTracker.DAL.Context;

public static class InMemoryStorage
{
    public static List<Transaction> Transactions { get; } = new();
    
    public static List<Category> Categories { get; } = new()
    {
        new Category(Guid.NewGuid(), "Salary", TransactionType.Income),
        new Category(Guid.NewGuid(), "Food", TransactionType.Expense),
        new Category(Guid.NewGuid(), "Transport", TransactionType.Expense)
    };
}