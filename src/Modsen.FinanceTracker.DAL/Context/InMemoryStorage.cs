using Modsen.FinanceTracker.Domain.Entities;

namespace Modsen.FinanceTracker.DAL.Context;

public static class InMemoryStorage
{
    public static List<Transaction> Transactions { get; } = new();

    public static List<Category> Categories { get; } = new();
}