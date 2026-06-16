namespace Modsen.FinanceTracker.UI.Menu;

public sealed class MainMenu : IMainMenu
{
    public string ShowAndGetChoice(IEnumerable<string> choices)
    {
        AnsiConsole.Clear();

        var rule = new Rule(Constants.MainMenu.MenuHeader);
        rule.Justification = Justify.Left;
        AnsiConsole.Write(rule);

        AnsiConsole.WriteLine();

        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(Constants.MainMenu.SelectOptionPrompt)
                .PageSize(Constants.MainMenu.MainPageSize)
                .AddChoices(choices));
    }
}