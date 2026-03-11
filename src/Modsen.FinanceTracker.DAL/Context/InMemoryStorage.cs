using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.Domain.Interfaces;

namespace Modsen.FinanceTracker.DAL.Context;

public class InMemoryStorage : IDataContext
{
    public List<Transaction> Transactions { get; } = new();

    public List<Category> Categories { get; } = new();
}