using Microsoft.Extensions.DependencyInjection;
using Modsen.FinanceTracker.BLL.Factories;
using Modsen.FinanceTracker.BLL.Interfaces;
using Modsen.FinanceTracker.BLL.Services;
using Modsen.FinanceTracker.BLL.Validators;
using Modsen.FinanceTracker.DAL;
using Modsen.FinanceTracker.DAL.Context;
using Modsen.FinanceTracker.DAL.Repositories;
using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.Domain.Interfaces;
using Modsen.FinanceTracker.Infrastructure.Configuration;
using Modsen.FinanceTracker.UI.Actions;
using Modsen.FinanceTracker.UI.Actions.TransactionActions;
using Modsen.FinanceTracker.UI.Actions.WalletActions;
using Modsen.FinanceTracker.UI.Interfaces;
using Modsen.FinanceTracker.UI.Menu;
using Modsen.FinanceTracker.UI.Views;
using Spectre.Console;

namespace Modsen.FinanceTracker.UI;

internal class Program
{
    private static async Task Main(string[] args)
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
            AppConfiguration config = AppConfiguration.Instance;
            var services = new ServiceCollection();

            services.AddSingleton(sp => new JsonDbContext(config.JsonDbPath));
            services.AddSingleton<IDataContext>(sp => sp.GetRequiredService<JsonDbContext>());

            services.AddSingleton<ICategoryRepository, JsonCategoryRepository>();
            services.AddSingleton<IWalletRepository, JsonWalletRepository>();
            services.AddSingleton<IUnitOfWork, UnitOfWork>();

            services.AddSingleton<IValidator<Transaction>, TransactionValidator>();
            services.AddSingleton<IFinanceService, FinanceService>();
            services.AddSingleton<ICategoryService, CategoryService>();
            services.AddSingleton<IReportService, ReportService>();
            services.AddSingleton<IWalletService, WalletService>();

            services.AddTransient<ITransactionFactory, TransactionFactory>();

            services.AddTransient<ITransactionListView, TransactionListView>();
            services.AddTransient<IAnalyticsListView, AnalyticsListView>();
            services.AddTransient<IWalletListView, WalletListView>();

            services.AddTransient<IWalletMenuAction, ViewWalletsAction>();
            services.AddTransient<IWalletMenuAction, CreateWalletAction>();
            services.AddTransient<IWalletMenuAction, DeleteWalletAction>();
            services.AddTransient<IMenuAction, ManageWalletsAction>();

            services.AddTransient<ITransactionMenuAction, ViewTransactionAction>();
            services.AddTransient<ITransactionMenuAction, AddTransactionAction>();
            services.AddTransient<ITransactionMenuAction, UpdateTransactionAction>();
            services.AddTransient<ITransactionMenuAction, DeleteTransactionAction>();
            services.AddTransient<IMenuAction, ManageTransactionsAction>();

            services.AddTransient<IMenuAction, CheckBalanceAction>();
            services.AddTransient<IMenuAction, AnalyticsAction>();

            services.AddTransient<IMenuAction, ExportReportAction>();
            services.AddTransient<IMenuAction, ExitAction>();

            services.AddSingleton<IMainMenu, MainMenu>();
            services.AddSingleton<IApp, App>();

            ServiceProvider serviceProvider = services.BuildServiceProvider();

            JsonDbContext context = serviceProvider.GetRequiredService<JsonDbContext>();
            await context.LoadAsync(cts.Token);

            ICategoryRepository categoryRepo = serviceProvider.GetRequiredService<ICategoryRepository>();
            await categoryRepo.SeedAsync(cts.Token);

            IApp app = serviceProvider.GetRequiredService<IApp>();
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