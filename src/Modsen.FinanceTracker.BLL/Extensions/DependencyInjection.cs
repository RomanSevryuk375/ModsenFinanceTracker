using Microsoft.Extensions.DependencyInjection;
using Modsen.FinanceTracker.BLL.Factories;
using Modsen.FinanceTracker.BLL.Interfaces;
using Modsen.FinanceTracker.BLL.Services;
using Modsen.FinanceTracker.BLL.Validators;
using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.Domain.Interfaces;

namespace Modsen.FinanceTracker.BLL.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddBLL(this IServiceCollection services, string currency)
    {
        services.AddSingleton<IValidator<Transaction>, TransactionValidator>();
        services.AddSingleton<IFinanceService, FinanceService>(sp => new FinanceService(
            sp.GetRequiredService<IWalletRepository>(),
            sp.GetRequiredService<IValidator<Transaction>>(),
            sp.GetRequiredService<ICurrencyService>(),
            sp.GetRequiredService<IUnitOfWork>(),
            currency));

        services.AddSingleton<ICategoryService, CategoryService>();
        services.AddSingleton<IReportService, ReportService>();
        services.AddSingleton<IWalletService, WalletService>();
        services.AddSingleton<ISchedulerService, SchedulerService>();

        services.AddTransient<ITransactionFactory, TransactionFactory>();

        return services;
    }
}
