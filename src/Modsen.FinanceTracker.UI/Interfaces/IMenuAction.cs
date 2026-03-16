namespace Modsen.FinanceTracker.UI.Interfaces;

public interface IMenuAction
{
    string Name { get; }
    Task ExecuteAsync(CancellationToken ct); 
}