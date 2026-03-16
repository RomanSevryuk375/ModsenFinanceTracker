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

    public async Task RunAsync(CancellationToken ct = default)
    {
        while (_isRunning && !ct.IsCancellationRequested)
        {
            var availableChoices = _actions.Select(a => a.Name);
            var choice = _mainMenu.ShowAndGetChoice(availableChoices);
            
            if (ct.IsCancellationRequested)
            {
                break;
            }

            HandleChoice(choice, ct);
        }
    }

    private void HandleChoice(string choice,  CancellationToken ct)
    {
        var action = _actions.FirstOrDefault(a => a.Name == choice);

        if (action is not null)
        {
            action.ExecuteAsync(ct);
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey(true);
    }
}