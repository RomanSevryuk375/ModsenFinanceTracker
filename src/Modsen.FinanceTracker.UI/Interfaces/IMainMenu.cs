namespace Modsen.FinanceTracker.UI.Interfaces;

public interface IMainMenu
{
    string ShowAndGetChoice(IEnumerable<string> choices);
}