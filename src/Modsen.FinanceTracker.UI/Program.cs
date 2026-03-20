using Modsen.FinanceTracker.BLL.Factories;
using Modsen.FinanceTracker.BLL.Services;
using Modsen.FinanceTracker.BLL.Validators;
using Modsen.FinanceTracker.DAL.Context;
using Modsen.FinanceTracker.DAL.Repositories;
using Modsen.FinanceTracker.Infrastructure.Configuration;
using Modsen.FinanceTracker.UI.Actions;
using Modsen.FinanceTracker.UI.Interfaces;
using Modsen.FinanceTracker.UI.Menu;
using Modsen.FinanceTracker.UI.Views;
using Spectre.Console;

namespace Modsen.FinanceTracker.UI;

class Program
{
    static async Task Main(string[] args)
    {
        using var cts = new CancellationTokenSource();
        
        Console.CancelKeyPress += (s, e) =>
        {
            e.Cancel = true; 
            cts.Cancel();  
            Console.WriteLine("\nCancellation requested...");
        };

        try
        {
            var config = AppConfiguration.Instance;
        
            var context = new JsonDbContext(config.JsonDbPath);
            await context.LoadAsync(cts.Token);
        
            var categoryRepo = new JsonCategoryRepository(context);
            await categoryRepo.SeedAsync(cts.Token); 
            var transactionRepo = new JsonTransactionRepository(context);

            var transactionValidator = new TransactionValidator();
        
            var financeService = new FinanceService(transactionRepo, transactionValidator);
            var categoryService = new CategoryService(categoryRepo);
            var reportService = new ReportService(transactionRepo);

            var transactionFactory = new TransactionFactory();

            var transactionListView = new TransactionListView();
            
            var actions = new List<IMenuAction>
            {
                new AddTransactionAction(financeService, transactionFactory, categoryService),
                new UpdateTransactionAction(financeService),
                new CheckBalanceAction(financeService),
                new DeleteTransactionAction(financeService),
                new ViewTransactionAction(financeService, categoryService, transactionListView),
                new ExportReportAction(reportService),
                new ExitAction()
            };

            var menu = new MainMenu();
        
            var app = new App(menu, actions);
            await app.RunAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            AnsiConsole.MarkupLine("Application stopped");
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
        }
        finally
        {
            AnsiConsole.MarkupLine("Exiting...");
        }
    }
}