namespace Modsen.FinanceTracker.UI.Interfaces;

public interface IMenuAction
{
    string Name { get; }
    void Execute();
}