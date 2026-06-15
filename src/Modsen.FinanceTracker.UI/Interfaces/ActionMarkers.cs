namespace Modsen.FinanceTracker.UI.Interfaces;

public interface IMenuAction
{
    public string Name { get; }
    public Task ExecuteAsync(CancellationToken cancellationToken);
}

public interface IWalletMenuAction : IMenuAction { }
public interface ITransactionMenuAction : IMenuAction { }
