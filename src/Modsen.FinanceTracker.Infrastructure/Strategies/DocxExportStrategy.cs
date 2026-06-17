using Modsen.FinanceTracker.BLL.Constants;
using Modsen.FinanceTracker.BLL.Interfaces;
using Modsen.FinanceTracker.Domain.Entities;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace Modsen.FinanceTracker.Infrastructure.Strategies;

public sealed class DocxExportStrategy : IExportStrategy
{
    public Task ExportAsync(
        IEnumerable<Transaction> transactions, 
        string filePath, 
        CancellationToken cancellationToken)
    {
        var transactionList = transactions.ToList();

        using var document = DocX.Create(filePath);

        document.InsertParagraph(ReportConstants.ReportTitle)
            .FontSize(ReportConstants.Docx.HeaderFontSize)
            .Bold()
            .SpacingAfter(ReportConstants.Docx.HeaderSpacingAfter);

        Table table = document.AddTable(transactionList.Count + 1, 5);
        table.Design = TableDesign.LightGridAccent1;

        string[] headers =
        {
            ReportConstants.Headers.Date,
            ReportConstants.Headers.Type,
            ReportConstants.Headers.Category,
            ReportConstants.Headers.Description,
            ReportConstants.Headers.Amount
        };

        for (int i = 0; i < headers.Length; i++)
        {
            table.Rows[0].Cells[i].Paragraphs.First().Append(headers[i]).Bold();
        }

        for (int i = 0; i < transactionList.Count; i++)
        {
            Transaction t = transactionList[i];
            int rowIndex = i + 1;

            string type = t is IncomeTransaction 
                ? ReportConstants.Types.Income 
                : ReportConstants.Types.Expense;
            string sign = t is IncomeTransaction 
                ? ReportConstants.Types.IncomeSign 
                : ReportConstants.Types.ExpenseSign;
            string categoryName = t.Category?.Name 
                ?? ReportConstants.NotAvailable;

            table.Rows[rowIndex].Cells[0].Paragraphs.First().Append(t.Date.Value.ToShortDateString());
            table.Rows[rowIndex].Cells[1].Paragraphs.First().Append(type);
            table.Rows[rowIndex].Cells[2].Paragraphs.First().Append(categoryName);
            table.Rows[rowIndex].Cells[3].Paragraphs.First().Append(t.Description.Value);
            table.Rows[rowIndex].Cells[4].Paragraphs.First().Append($"{sign}{t.Amount:N2}");
        }

        document.InsertTable(table);
        document.Save();

        return Task.CompletedTask;
    }
}
