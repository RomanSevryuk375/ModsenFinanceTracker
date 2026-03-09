using Modsen.FinanceTracker.BLL.Services;
using Modsen.FinanceTracker.DAL.Repositories;
using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.UI.Menu;
using Modsen.FinanceTracker.UI.Views;
using Spectre.Console;

namespace Modsen.FinanceTracker.UI;

public class App
{
    private readonly MainMenu _mainMenu;
    private readonly FinanceService _financeService;
    private readonly TransactionListView _listView;
    private bool _isRunning = true;

    public App()
    {
        var repo = new TransactionRepository();
        _financeService = new FinanceService(repo);
        _mainMenu = new MainMenu();
        _listView = new TransactionListView();
    }

    public void Run()
    {
        while (_isRunning)
        {
            var choice = _mainMenu.ShowAndGetChoice();
            HandleChoice(choice);
        }
    }

    private void HandleChoice(string choice)
    {
        switch (choice)
        {
            case "Add Transaction":
                var amount = AnsiConsole.Ask<decimal>("Enter amount:");
                var desc = AnsiConsole.Ask<string>("Enter description:");
                var type = AnsiConsole.Confirm("Is it Income?") ? "in" : "out";
                
                Transaction newT = type == "in" 
                    ? new IncomeTransaction(Guid.NewGuid(), amount, desc, DateTime.Now, Guid.Empty)
                    : new ExpenseTransaction(Guid.NewGuid(), amount, desc, DateTime.Now, Guid.Empty);
                
                _financeService.AddTransaction(newT);
                break;

            case "View History":
                var transactions = _financeService.GetFilteredTransactions(null, null, null);
                _listView.Render(transactions);
                break;

            case "Delete Transaction":
                var id = AnsiConsole.Ask<Guid>("Enter id:");
                _financeService.DeleteTransaction(id);
                break;
            
            case "Check Balance":
                var balance = _financeService.GetBalance();
                AnsiConsole.MarkupLine($"[bold]Current Balance:[/] [green]{balance:C}[/]");
                break;

            case "Exit":
                _isRunning = false;
                return;
        }
        AnsiConsole.WriteLine("\nPress any key to return...");
        Console.ReadKey(true);
    }
}