using Modsen.FinanceTracker.UI.Interfaces;
using Modsen.FinanceTracker.UI.Models;
using Spectre.Console;

namespace Modsen.FinanceTracker.UI.Views;

public sealed class AnalyticsListView : IAnalyticsListView
{
    public void Render(IEnumerable<AnalyticsRowModel> rows)
    {
        var rowList = rows.ToList();

        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title($"[{Constants.Colors.Primary}]{Constants.Tables.AnalyticsTitle}[/]")
            .LeftAligned();

        table.AddColumn($"{Constants.Tables.Category}");
        table.AddColumn($"{Constants.Tables.Amount}");
        table.AddColumn($"{Constants.Tables.Percent}");

        foreach (var row in rowList)
        {
            table.AddRow(row.CategoryName, row.FormattedAmount, row.FormattedPercent);
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();

        var chart = new BreakdownChart()
            .FullSize()
            .Width(Constants.BreakdownChart.Width);

        var colors = new[] 
        { 
            Color.Red, 
            Color.Blue, 
            Color.Green, 
            Color.Yellow, 
            Color.Orange3, 
            Color.Purple 
        };

        var colorIndex = 0;

        foreach (var row in rowList)
        {
            chart.AddItem(row.CategoryName, row.RawAmount, colors[colorIndex % colors.Length]);
            colorIndex++;
        }

        AnsiConsole.Write(new Panel(chart)
        {
            Header = new PanelHeader(Constants.BreakdownChart.Name),
            Border = BoxBorder.Rounded,
            Padding = new Padding(
                Constants.BreakdownChart.PaddingLeft,
                Constants.BreakdownChart.PaddingTop,
                Constants.BreakdownChart.PaddingRight,
                Constants.BreakdownChart.PaddingBottom)
        });
    }
}