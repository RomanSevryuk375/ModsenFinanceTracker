using Modsen.FinanceTracker.Domain.Entities;

namespace Modsen.FinanceTracker.Domain.Interfaces;

public interface IDataContext
{
    public List<Wallet> Wallets { get; }
    public List<Category> Categories { get; }
}