namespace Modsen.FinanceTracker.UI.Interfaces;

public interface IApp
{
    public Task RunAsync(CancellationToken cancellationToken = default);
}