using System.Text;
using Modsen.FinanceTracker.BLL.Constants;
using Modsen.FinanceTracker.BLL.Interfaces;
using Modsen.FinanceTracker.Domain.Entities;

namespace Modsen.FinanceTracker.Infrastructure.Strategies;

public sealed class CsvExportStrategy : IExportStrategy
{
    public async Task ExportAsync(
        IEnumerable<Transaction> transactions, 
        string filePath, 
        CancellationToken cancellationToken)
    {
        var csv = new StringBuilder();
        csv.AppendLine(ReportConstants.Headers.CsvHeader);

        foreach (Transaction t in transactions)
        {
            string type = t is IncomeTransaction 
                ? ReportConstants.Types.Income 
                : ReportConstants.Types.Expense;
            string categoryName = t.Category?.Name 
                ?? ReportConstants.NotAvailable;

            csv.AppendLine($"{t.Date:d},{type},{categoryName},\"{t.Description}\",{t.Amount}");
        }

        await File.WriteAllTextAsync(filePath, csv.ToString(), cancellationToken);
    }
}
