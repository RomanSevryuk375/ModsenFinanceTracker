using Modsen.FinanceTracker.UI.Interfaces;
using Modsen.FinanceTracker.UI.Models;
using Spectre.Console;

namespace Modsen.FinanceTracker.UI.Views;

public sealed class TransactionListView : ITransactionListView
{
    public void Render(IEnumerable<TransactionRowModel> rows)
    {
        var rowList = rows.ToList();

        var pageSize = Constants.Tables.PageSize;

        var totalPages = (int)Math.Ceiling((double)rowList.Count / pageSize);
        var currentPage = 0;

        bool keepPaginating = true;

        while (keepPaginating)
        {
            AnsiConsole.Clear();

            var chunk = rowList.Skip(currentPage * pageSize).Take(pageSize);

            var table = new Table()
                .Border(TableBorder.Rounded)
                .Title($"[{Constants.Colors.Primary}]" +
                $"{Constants.Tables.Title} (Page {currentPage + 1} of {totalPages})[/]")
                .LeftAligned();

            table.AddColumn($"{Constants.Tables.Id}");
            table.AddColumn($"{Constants.Tables.Date}");
            table.AddColumn($"{Constants.Tables.Category}");
            table.AddColumn($"{Constants.Tables.Description}");
            table.AddColumn($"{Constants.Tables.Amount}");

            foreach (var row in chunk)
            {
                table.AddRow(
                    row.Id,
                    row.Date,
                    row.CategoryName,  
                    row.Description,
                    row.FormattedAmount);
            }

            AnsiConsole.Write(table);

            if (totalPages <= 1)
            {
                break;
            }

            var choices = new List<string>();
            const string next = Constants.Tables.NextPageButton;
            const string prev = Constants.Tables.PrevPageButton;
            const string exit = Constants.Tables.ExitButton;

            if (currentPage > 0)
            {
                choices.Add(prev);
            }

            if (currentPage < totalPages - 1)
            {
                choices.Add(next);
            }

            choices.Add(exit);

            AnsiConsole.WriteLine();
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title(Constants.Tables.NavigationTitle)
                    .AddChoices(choices));

            if (choice == next)
            {
                currentPage++;
            }
            else if (choice == prev)
            {
                currentPage--;
            }
            else
            {
                keepPaginating = false;
            }
        }
    }
}