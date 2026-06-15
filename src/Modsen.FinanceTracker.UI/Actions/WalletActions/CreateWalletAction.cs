using Modsen.FinanceTracker.BLL.Interfaces;
using Modsen.FinanceTracker.Domain;
using Modsen.FinanceTracker.UI.Helpers;
using Modsen.FinanceTracker.UI.Interfaces;
using Spectre.Console;

namespace Modsen.FinanceTracker.UI.Actions.WalletActions;

public sealed class CreateWalletAction(
    IWalletService walletService) : IWalletMenuAction
{
    public string Name => Constants.MainMenu.CreateWallet;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        string name = AnsiConsole.Ask<string>(Constants.Prompts.WalletName);
        string currency = PromptCurrency();

        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        Result result = await walletService.CreateWalletAsync(name, currency, cancellationToken);

        UIHelper.ProcessResult(result, Constants.Success.CreateWallet);
    }

    private static string PromptCurrency()
    {
        return AnsiConsole.Prompt(
            new TextPrompt<string>(Constants.Prompts.WalletCurrency)
                .Validate(c => c.Trim().Length == Constants.UI.CurrencyLength
                    ? ValidationResult.Success()
                    : ValidationResult.Error(Constants.Errors.CurrencyLength)));
    }
}