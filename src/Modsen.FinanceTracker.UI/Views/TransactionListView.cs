using Modsen.FinanceTracker.UI.Interfaces;
using Modsen.FinanceTracker.UI.Models;
using Spectre.Console;

namespace Modsen.FinanceTracker.UI.Views;

public sealed class TransactionListView : ITransactionListView
{
    public void Render(IEnumerable<TransactionRowModel> rows)
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title($"[{Constants.Colors.Primary}]{Constants.Tables.Title}[/]")
            .LeftAligned();

        table.AddColumn($"{Constants.Tables.Id}");
        table.AddColumn($"{Constants.Tables.Date}");
        table.AddColumn($"{Constants.Tables.Category}");
        table.AddColumn($"{Constants.Tables.Description}");
        table.AddColumn($"{Constants.Tables.Amount}");

        foreach (var row in rows)
        {
            table.AddRow(row.Id, row.Date, row.Description, row.CategoryName, row.FormattedAmount);
        }

        AnsiConsole.Write(table);
    }
}