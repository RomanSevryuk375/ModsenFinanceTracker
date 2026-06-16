namespace Modsen.FinanceTracker.UI.Views;

public sealed class TransactionListView : ITransactionListView
{
    public void Render(IEnumerable<TransactionRowModel> rows)
    {
        var rowList = rows.ToList();

        int pageSize = Constants.Tables.PageSize;

        int totalPages = (int)Math.Ceiling((double)rowList.Count / pageSize);
        int currentPage = 0;

        bool keepPaginating = true;

        while (keepPaginating)
        {
            AnsiConsole.Clear();

            IEnumerable<TransactionRowModel> chunk = rowList.Skip(currentPage * pageSize).Take(pageSize);

            Table table = new Table()
                .Border(TableBorder.Rounded)
                .Title($"[{Constants.Colors.Primary}]" +
                $"{Constants.Tables.Title} (Page {currentPage + 1} of {totalPages})[/]")
                .LeftAligned();

            table.AddColumn($"{Constants.Tables.Id}");
            table.AddColumn($"{Constants.Tables.Date}");
            table.AddColumn($"{Constants.Tables.Category}");
            table.AddColumn($"{Constants.Tables.Description}");
            table.AddColumn($"{Constants.Tables.Amount}");

            foreach (TransactionRowModel? row in chunk)
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
            const string Next = Constants.Tables.NextPageButton;
            const string Prev = Constants.Tables.PrevPageButton;
            const string Exit = Constants.Tables.ExitButton;

            if (currentPage > 0)
            {
                choices.Add(Prev);
            }

            if (currentPage < totalPages - 1)
            {
                choices.Add(Next);
            }

            choices.Add(Exit);

            AnsiConsole.WriteLine();
            string choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title(Constants.Tables.NavigationTitle)
                    .AddChoices(choices));

            if (choice == Next)
            {
                currentPage++;
            }
            else if (choice == Prev)
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