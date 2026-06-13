using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.Domain.Interfaces;

namespace Modsen.FinanceTracker.DAL.Context;

public sealed record InMemoryStorage : IDataContext
{
    public List<Transaction> Transactions { get; } = [];
    public List<Category> Categories { get; } = [];
}