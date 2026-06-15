using Modsen.FinanceTracker.Domain.Entities;

namespace Modsen.FinanceTracker.UI.Interfaces;

public interface IWalletListView
{
    public void Render(IReadOnlyList<Wallet> wallets);
}