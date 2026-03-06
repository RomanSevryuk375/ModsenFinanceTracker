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
            case "Add Transaction":
                AnsiConsole.MarkupLine("[blue]Adding logic will be here (Task 3)[/]");
                break;
            case "View History":
                AnsiConsole.MarkupLine("[blue]History view will be here (Task 3)[/]");
                break;
            case "Delete Transaction":
                AnsiConsole.MarkupLine("[blue]Delete logic will be here (Task 3)[/]");
                break;
            case "Check Balance":
                AnsiConsole.MarkupLine("[green]Current Balance: Mock[/]");
                break;
            case "Exit":
                _isRunning = false;
                AnsiConsole.MarkupLine("[bold red]Exiting [/]");
                return;
        }

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
        Console.ReadKey(true);
    }
}