using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Modsen.FinanceTracker.BLL.Strategies;

public sealed class PdfExportStrategy : IExportStrategy
{
    public PdfExportStrategy()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public Task ExportAsync(
        IEnumerable<Transaction> transactions, 
        string filePath, 
        CancellationToken cancellationToken)
    {
        var transactionList = transactions.ToList();

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(ReportConstants.Pdf.MarginCentimetres, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(ReportConstants.Pdf.DefaultFontSize));

                page.Header()
                    .Text(ReportConstants.ReportTitle)
                    .SemiBold()
                    .FontSize(ReportConstants.Pdf.HeaderFontSize)
                    .FontColor(Colors.Blue.Darken2);

                page.Content()
                .PaddingVertical(ReportConstants.Pdf.ContentPaddingVerticalCentimetres, Unit.Centimetre).Column(x =>
                {
                    x.Spacing(ReportConstants.Pdf.ContentSpacing);

                    x.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(ReportConstants.Pdf.ColDateWidth);
                            columns.ConstantColumn(ReportConstants.Pdf.ColTypeWidth);
                            columns.ConstantColumn(ReportConstants.Pdf.ColCategoryWidth);
                            columns.RelativeColumn();
                            columns.ConstantColumn(ReportConstants.Pdf.ColAmountWidth);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text(ReportConstants.Headers.Date).SemiBold();
                            header.Cell().Text(ReportConstants.Headers.Type).SemiBold();
                            header.Cell().Text(ReportConstants.Headers.Category).SemiBold();
                            header.Cell().Text(ReportConstants.Headers.Description).SemiBold();
                            header.Cell().AlignRight().Text(ReportConstants.Headers.Amount).SemiBold();

                            header.Cell().ColumnSpan(5)
                                .PaddingTop(ReportConstants.Pdf.CellPadding)
                                .BorderBottom(ReportConstants.Pdf.BorderBottomThickness)
                                .BorderColor(Colors.Black);
                        });

                        foreach (Transaction t in transactionList)
                        {
                            string type = t is IncomeTransaction 
                                ? ReportConstants.Types.Income 
                                : ReportConstants.Types.Expense;
                            string color = t is IncomeTransaction 
                                ? Colors.Green.Medium
                                : Colors.Red.Medium;
                            string sign = t is IncomeTransaction
                                ? ReportConstants.Types.IncomeSign 
                                : ReportConstants.Types.ExpenseSign;
                            string categoryName = t.Category?.Name 
                                ?? ReportConstants.NotAvailable;

                            table.Cell()
                                .PaddingVertical(ReportConstants.Pdf.CellPadding)
                                .Text(t.Date.Value.ToShortDateString());
                            table.Cell()
                                .PaddingVertical(ReportConstants.Pdf.CellPadding)
                                .Text(type).FontColor(color);
                            table.Cell()
                                .PaddingVertical(ReportConstants.Pdf.CellPadding
                                ).Text(categoryName);
                            table.Cell()
                                .PaddingVertical(ReportConstants.Pdf.CellPadding)
                                .Text(t.Description.Value);
                            table.Cell()
                                .PaddingVertical(ReportConstants.Pdf.CellPadding)
                                .AlignRight().Text($"{sign}{t.Amount:N2}").FontColor(color);
                        }
                    });
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span(ReportConstants.Pdf.PageText);
                    x.CurrentPageNumber();
                    x.Span(ReportConstants.Pdf.OfText);
                    x.TotalPages();
                });
            });
        })
        .GeneratePdf(filePath);

        return Task.CompletedTask;
    }
}
