namespace Modsen.FinanceTracker.DAL.Context;

public sealed record InMemoryStorage : IDataContext
{
    public List<Wallet> Wallets { get; } = [];
    public List<Category> Categories { get; } = [];
}
