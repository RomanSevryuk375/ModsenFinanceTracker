using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.Domain.Enums;
using Modsen.FinanceTracker.UI.Interfaces;
using Spectre.Console;

namespace Modsen.FinanceTracker.UI.Views;

public sealed class TemplateListView : ITemplateListView
{
    public void Render(IReadOnlyList<RecurringTransactionTemplate> templates)
    {
        Table table = new Table()
            .Border(TableBorder.Rounded)
            .Title($"[{Constants.Colors.Primary}]{Constants.Tables.TemplatesTitle}[/]")
            .LeftAligned();

        table.AddColumn(Constants.Tables.TemplateName);
        table.AddColumn(Constants.Tables.Category);
        table.AddColumn(Constants.Tables.Amount);
        table.AddColumn(Constants.Tables.Period);
        table.AddColumn(Constants.Tables.NextDate);

        foreach (RecurringTransactionTemplate t in templates)
        {
            string color = t.Category.Type == TransactionType.Income
                ? Constants.Colors.Success
                : Constants.Colors.Error;

            string sign = t.Category.Type == TransactionType.Income
                ? "+"
                : "-";

            table.AddRow(
                t.Name,
                t.Category.Name,
                $"[{color}]{sign}{t.Amount:N2}[/]",
                t.Period.ToString(),
                t.NextExecutionDate.ToShortDateString()
            );
        }

        AnsiConsole.Write(table);
    }
}
