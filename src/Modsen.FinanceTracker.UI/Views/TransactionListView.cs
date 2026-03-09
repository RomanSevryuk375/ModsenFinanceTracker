using Modsen.FinanceTracker.Domain.Entities;
using Spectre.Console;

namespace Modsen.FinanceTracker.UI.Views;

public class TransactionListView
{
    public void Render(IEnumerable<Transaction> transactions)
    {
        var table = new Table().Border(TableBorder.Rounded);
        table.AddColumn("[yellow]Id[/]");
        table.AddColumn("[yellow]Date[/]");
        table.AddColumn("[yellow]Category[/]");
        table.AddColumn("[yellow]Description[/]");
        table.AddColumn("[yellow]Amount[/]");

        foreach (var t in transactions)
        {
            string color = t is IncomeTransaction ? "green" : "red";
            string sign = t is IncomeTransaction ? "+" : "-";
            
            table.AddRow(
                t.Id.ToString(),
                t.Date.ToShortDateString(),
                "Mock",
                t.Description,
                $"[{color}]{sign}{t.Amount:C}[/]"
            );
        }

        AnsiConsole.Write(table);
    }
}