namespace Modsen.FinanceTracker.UI.Views;

public sealed class WalletListView : IWalletListView
{
    public void Render(IReadOnlyList<Wallet> wallets)
    {
        Table table = new Table()
            .Border(TableBorder.Rounded)
            .Title($"[{Constants.Colors.Primary}]{Constants.Tables.WalletsTitle}[/]")
            .LeftAligned();

        table.AddColumn(Constants.Tables.Id);
        table.AddColumn(Constants.Tables.WalletName);
        table.AddColumn(Constants.Tables.WalletCurrency);
        table.AddColumn(Constants.Tables.WalletBalance);

        foreach (Wallet wallet in wallets)
        {
            string balanceColor = wallet.Balance.Amount >= 0
                ? Constants.Colors.Success
                : Constants.Colors.Error;

            table.AddRow(
                wallet.Id.ToString()[..Constants.UI.GuidShortLength] + "...",
                wallet.Name,
                wallet.BaseCurrency,
                $"[{balanceColor}]{wallet.Balance:N2}[/]"
            );
        }

        AnsiConsole.Write(table);
    }
}
