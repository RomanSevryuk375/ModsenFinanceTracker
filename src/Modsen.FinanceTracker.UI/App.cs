using Modsen.FinanceTracker.UI.Menu;
using Spectre.Console;

namespace Modsen.FinanceTracker.UI;

public class App
{
    private readonly MainMenu _mainMenu;
    private bool _isRunning;

    public App()
    {
        _mainMenu = new MainMenu();
        _isRunning = true;
    }

    public void Run()
    {
        while (_isRunning)
        {
            var choice = _mainMenu.ShowAndGetChoice();
            HandleChoice(choice);
        }
    }

    private void HandleChoice(string choice)
    {
        switch (choice)
        {
            case Constants.MainMenu.ActionAdd:
                AnsiConsole.MarkupLine($"[{Constants.Colors.Info}]Adding logic will be here (Task 3)[/]");
                break;
            case Constants.MainMenu.ActionView:
                AnsiConsole.MarkupLine($"[{Constants.Colors.Info}]History view will be here (Task 3)[/]");
                break;
            case Constants.MainMenu.ActionDelete:
                AnsiConsole.MarkupLine($"[{Constants.Colors.Info}]Delete logic will be here (Task 3)[/]");
                break;
            case Constants.MainMenu.ActionBalance:
                AnsiConsole.MarkupLine($"[{Constants.Colors.Success}]Current Balance: Mock[/]");
                break;
            case Constants.MainMenu.ActionExit:
                _isRunning = false;
                AnsiConsole.MarkupLine($"[{Constants.Colors.Error}]Exiting [/]");
                return;
        }

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"[{Constants.Colors.Wait}]Press any key to continue...[/]");
        Console.ReadKey(true);
    }
}