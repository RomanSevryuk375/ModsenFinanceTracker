using Modsen.FinanceTracker.UI.Interfaces;

namespace Modsen.FinanceTracker.UI.Actions;

public class ExitAction : IMenuAction
{
    public string Name => Constants.MainMenu.ActionExit;
    public void Execute() 
    {
        Environment.Exit(0);
    }
}