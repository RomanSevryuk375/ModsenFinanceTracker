using Microsoft.Extensions.DependencyInjection;
using Modsen.FinanceTracker.DAL.Context;
using Modsen.FinanceTracker.Domain.Interfaces;
using Modsen.FinanceTracker.UI.Actions;
using Modsen.FinanceTracker.UI.Actions.TemplateActions;
using Modsen.FinanceTracker.UI.Actions.TransactionActions;
using Modsen.FinanceTracker.UI.Actions.WalletActions;
using Modsen.FinanceTracker.UI.Menu;
using Modsen.FinanceTracker.UI.Views;

namespace Modsen.FinanceTracker.UI.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddUi(this IServiceCollection services, string currency)
    {
        services.AddTransient<ITransactionListView, TransactionListView>();
        services.AddTransient<IAnalyticsListView, AnalyticsListView>();
        services.AddTransient<IWalletListView, WalletListView>();
        services.AddTransient<ITemplateListView, TemplateListView>();

        services.AddTransient<IWalletMenuAction, ViewWalletsAction>();
        services.AddTransient<IWalletMenuAction, CreateWalletAction>();
        services.AddTransient<IWalletMenuAction, DeleteWalletAction>();
        services.AddTransient<IMenuAction, ManageWalletsAction>();

        services.AddTransient<ITransactionMenuAction, ViewTransactionAction>();
        services.AddTransient<ITransactionMenuAction, AddTransactionAction>();
        services.AddTransient<ITransactionMenuAction, UpdateTransactionAction>();
        services.AddTransient<ITransactionMenuAction, DeleteTransactionAction>();
        services.AddTransient<IMenuAction, ManageTransactionsAction>();

        services.AddTransient<ITemplateMenuAction, AddTemplateAction>();
        services.AddTransient<ITemplateMenuAction, ViewTemplatesAction>();
        services.AddTransient<ITemplateMenuAction, DeleteTemplateAction>();
        services.AddTransient<IMenuAction, ManageTemplatesAction>();

        services.AddTransient<IMenuAction, CheckBalanceAction>(sp => new CheckBalanceAction(
            sp.GetRequiredService<IFinanceService>(),
            sp.GetRequiredService<IWalletService>(),
            currency));

        services.AddTransient<IMenuAction, AnalyticsAction>();

        services.AddTransient<IMenuAction, ExportReportAction>();
        services.AddTransient<IMenuAction, ExitAction>();

        services.AddSingleton<IMainMenu, MainMenu>();
        services.AddSingleton<IApp, App>();

        return services;
    }

    public static async Task InitApplicationAsync(
        this IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        JsonDbContext context = serviceProvider.GetRequiredService<JsonDbContext>();
        await context.LoadAsync(cancellationToken);

        ICategoryRepository categoryRepo = serviceProvider.GetRequiredService<ICategoryRepository>();
        await categoryRepo.SeedAsync(cancellationToken);

        IApp app = serviceProvider.GetRequiredService<IApp>();
        await app.RunAsync(cancellationToken);
    }
}
