using System.Text;
using Modsen.FinanceTracker.BLL.Interfaces;
using Modsen.FinanceTracker.Domain.Entities;

namespace Modsen.FinanceTracker.BLL.Strategies;

public class TxtExportStrategy : IExportStrategy
{
    public async Task ExportAsync(IEnumerable<Transaction> transactions, string filePath, CancellationToken ct)
    {
        var txt = new StringBuilder();
        txt.AppendLine(new string('-', 30));

        foreach (var t in transactions)
        {
            var type = t is IncomeTransaction ? "[+]" : "[-]";
            txt.AppendLine($"{t.Date:d} | {type} | {t.Amount:N2} | {t.Description}");
        }

        await File.WriteAllTextAsync(filePath, txt.ToString(), ct);
    }
}