using Modsen.FinanceTracker.Domain.Extensions;

namespace Modsen.FinanceTracker.UI.Helpers;

public static class UIHelper
{
    public static async Task<Wallet?> PromptWalletAsync(
        IWalletService walletService,
        CancellationToken cancellationToken)
    {
        Result<IReadOnlyList<Wallet>> result = await walletService.GetAllWallets(cancellationToken);
        if (result.IsFailure || result.Value.Count == 0)
        {
            AnsiConsole.MarkupLine(Constants.Errors.WalletNotFound);
            return null;
        }

        return AnsiConsole.Prompt(
            new SelectionPrompt<Wallet>()
                .Title(Constants.Prompts.WalletSelectionTitle)
                .UseConverter(w => $"{w.Name} ({w.BaseCurrency}) - Balance: {w.Balance:N2}")
                .AddChoices(result.Value));
    }

    public static void ProcessResult(Result result, string successMessage)
    {
        if (result.IsFailure)
        {
            AnsiConsole.MarkupLine($"[{Constants.Colors.Error}]Error: {result.Error}[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[{Constants.Colors.Success}]{successMessage}[/]");
        }
    }

    public static async Task<IReadOnlyList<Transaction>> FetchTransactionsAsync(
        Guid walletId,
        TransactionFilterDto filterDto,
        IFinanceService financeService,
        CancellationToken cancellationToken)
    {
        Result<IReadOnlyList<Transaction>> transResult = await financeService.GetFilteredTransactionsAsync(
            walletId, filterDto, cancellationToken);
        if (transResult.IsFailure || transResult.Value.Count == 0)
        {
            AnsiConsole.MarkupLine(Constants.Errors.TransactionNotFound);
            return [];
        }

        return transResult.Value;
    }
}