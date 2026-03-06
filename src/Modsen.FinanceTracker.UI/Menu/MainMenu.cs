using Spectre.Console;

namespace Modsen.FinanceTracker.UI.Menu;

public class MainMenu
{
    public string ShowAndGetChoice()
    {
        AnsiConsole.Clear();
        
        var rule = new Rule("[yellow]Main Menu[/]");
        rule.Justification = Justify.Left;
        AnsiConsole.Write(rule);

        AnsiConsole.WriteLine(); 

        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Please select an [green]option[/]:")
                .PageSize(10)
                .AddChoices(new[] {
                    "Add Transaction", 
                    "View History", 
                    "Delete Transaction", 
                    "Check Balance", 
                    "Exit"
                }));
    }
}