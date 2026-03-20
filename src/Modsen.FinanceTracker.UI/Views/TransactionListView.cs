using Modsen.FinanceTracker.UI.Interfaces;
using Modsen.FinanceTracker.UI.Models;
using Spectre.Console;

namespace Modsen.FinanceTracker.UI.Views;

public class TransactionListView : ITransactionListView
{
    public void Render(IEnumerable<TransactionRowModel> rows)
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title($"[{Constants.Colors.Primary}]{Constants.Tables.TransactionsTableTitle}[/]")
            .LeftAligned();

        table.AddColumn($"{Constants.Tables.TransactionsTableId}");
        table.AddColumn($"{Constants.Tables.TransactionsDate}");
        table.AddColumn($"{Constants.Tables.TransactionCategory}");
        table.AddColumn($"{Constants.Tables.TransactionDescription}");
        table.AddColumn($"{Constants.Tables.TransactionsAmount}");

        foreach (var row in rows)
        {
            table.AddRow(
                row.Id,
                row.Date,
                row.Description,
                row.CategoryName,
                row.FormattedAmount);
        }

        AnsiConsole.Write(table);
    }
}