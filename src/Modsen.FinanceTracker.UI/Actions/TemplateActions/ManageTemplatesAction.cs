using Modsen.FinanceTracker.UI.Interfaces;
using Spectre.Console;

namespace Modsen.FinanceTracker.UI.Actions.TemplateActions;

public sealed class ManageTemplatesAction(
    IEnumerable<ITemplateMenuAction> templateActions) : IMenuAction
{
    public string Name => Constants.MainMenu.ManageTemplates;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var choices = templateActions.Select(a => a.Name).ToList();
        choices.Add(Constants.MainMenu.BackToMainMenu);

        string choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(Constants.Prompts.TemplateManagementTitle)
                .AddChoices(choices));

        if (choice == Constants.MainMenu.BackToMainMenu)
        {
            return;
        }

        ITemplateMenuAction actionToExecute = templateActions.First(a => a.Name == choice);
        await actionToExecute.ExecuteAsync(cancellationToken);
    }
}
