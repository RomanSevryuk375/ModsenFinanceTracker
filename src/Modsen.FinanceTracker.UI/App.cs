using Modsen.FinanceTracker.UI.Interfaces;

namespace Modsen.FinanceTracker.UI;

public class App : IApp
{
    private readonly IMainMenu _mainMenu;
    private readonly IEnumerable<IMenuAction> _actions;
    private bool _isRunning = true;

    public App(IMainMenu mainMenu, IEnumerable<IMenuAction> actions)
    {
        _mainMenu = mainMenu;
        _actions = actions;
    }

    public async Task RunAsync()
    {
        while (_isRunning)
        {
            var availableChoices = _actions.Select(a => a.Name);
            var choice = _mainMenu.ShowAndGetChoice(availableChoices);

            HandleChoice(choice);
        }
    }

    private void HandleChoice(string choice)
    {
        var action = _actions.FirstOrDefault(a => a.Name == choice);

        if (action is not null)
        {
            action.Execute();
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey(true);

    }
}