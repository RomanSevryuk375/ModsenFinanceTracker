using System.Text;
using Modsen.FinanceTracker.BLL.Interfaces;
using Modsen.FinanceTracker.Domain.Entities;

namespace Modsen.FinanceTracker.BLL.Strategies;

public sealed class CsvExportStrategy : IExportStrategy
{
    public async Task ExportAsync(
        IEnumerable<Transaction> transactions,
        string filePath,
        CancellationToken cancellationToken)
    {
        const string tableHeader = "Date,Type,Amount,Description";

        var csv = new StringBuilder();
        csv.AppendLine($"{tableHeader}");

        foreach (var t in transactions)
        {
            var type = t is IncomeTransaction ? "Income" : "Expense";
            csv.AppendLine($"{t.Date:d},{type},{t.Amount},\"{t.Description}\"");
        }

        await File.WriteAllTextAsync(filePath, csv.ToString(), cancellationToken);
    }
}