namespace Modsen.FinanceTracker.BLL.Constants;

public static class ReportConstants
{
    public const string ReportTitle = "Finance Tracker Report";
    public const string NotAvailable = "N/A";

    public static class Headers
    {
        public const string Date = "Date";
        public const string Type = "Type";
        public const string Category = "Category";
        public const string Description = "Description";
        public const string Amount = "Amount";
        public const string CsvHeader = "Date,Type,Category,Description,Amount";
    }

    public static class Types
    {
        public const string Income = "Income";
        public const string Expense = "Expense";

        public const string IncomeSign = "+";
        public const string ExpenseSign = "-";

        public const string TxtIncome = "[+]";
        public const string TxtExpense = "[-]";
    }

    public static class Pdf
    {
        public const float MarginCentimetres = 2f;
        public const int DefaultFontSize = 11;
        public const int HeaderFontSize = 20;

        public const float ContentPaddingVerticalCentimetres = 1f;
        public const float ContentSpacing = 20f;

        public const float ColDateWidth = 80f;
        public const float ColTypeWidth = 60f;
        public const float ColCategoryWidth = 100f;
        public const float ColAmountWidth = 80f;

        public const float CellPadding = 5f;
        public const float BorderBottomThickness = 1f;

        public const string PageText = "Page ";
        public const string OfText = " of ";
    }

    public static class Docx
    {
        public const int HeaderFontSize = 18;
        public const double HeaderSpacingAfter = 15d;
    }

    public static class Txt
    {
        public const int SeparatorLength = 80;
        public const char SeparatorChar = '-';
        public const string ColumnSeparator = " | ";
    }
}
