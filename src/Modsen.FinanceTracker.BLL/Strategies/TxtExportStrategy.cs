using System.Text;

namespace Modsen.FinanceTracker.BLL.Strategies;

public sealed class TxtExportStrategy : IExportStrategy
{
    public async Task ExportAsync(
        IEnumerable<Transaction> transactions, 
        string filePath, 
        CancellationToken cancellationToken)
    {
        var txt = new StringBuilder();

        txt.AppendLine(ReportConstants.ReportTitle);
        txt.AppendLine(new string(ReportConstants.Txt.SeparatorChar, ReportConstants.Txt.SeparatorLength));

        foreach (Transaction t in transactions)
        {
            string type = t is IncomeTransaction 
                ? ReportConstants.Types.TxtIncome 
                : ReportConstants.Types.TxtExpense;
            string categoryName = t.Category?.Name 
                ?? ReportConstants.NotAvailable;
            string sep = ReportConstants.Txt.ColumnSeparator;

            txt.AppendLine($"{t.Date:d}{sep}{type}{sep}{categoryName}{sep}{t.Amount:N2}{sep}{t.Description}");
        }

        await File.WriteAllTextAsync(filePath, txt.ToString(), cancellationToken);
    }
}