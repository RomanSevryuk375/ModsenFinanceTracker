using Modsen.FinanceTracker.UI.Actions;
using Modsen.FinanceTracker.UI.Interfaces;
using Modsen.FinanceTracker.UI.Menu;

namespace Modsen.FinanceTracker.UI;

class Program
{
    static void Main(string[] args)
    {
        var actions = new List<IMenuAction>
        {
            new AddTransactionAction(),
            new CheckBalanceAction(),
            new DeleteTransectionAction(),
            new ViewTransactionAction(),
            new ExitAction(),
        };

        IMainMenu menu = new MainMenu();
        IApp app = new App(menu, actions);
        app.Run();
    }
}