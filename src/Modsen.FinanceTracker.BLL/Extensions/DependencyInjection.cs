using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Modsen.FinanceTracker.BLL.Decorators;
using Modsen.FinanceTracker.BLL.Factories;
using Modsen.FinanceTracker.BLL.Services;
using Modsen.FinanceTracker.BLL.Validators;

namespace Modsen.FinanceTracker.BLL.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddBLL(this IServiceCollection services, string currency)
    {
        services.AddSingleton<IValidator<Transaction>, TransactionValidator>();
        services.AddTransient<ITransactionFactory, TransactionFactory>();

        services.AddSingleton(sp => new FinanceService(
            sp.GetRequiredService<IWalletRepository>(),
            sp.GetRequiredService<IValidator<Transaction>>(),
            sp.GetRequiredService<ICurrencyService>(),
            sp.GetRequiredService<IUnitOfWork>(),
            currency));

        services.AddSingleton<CategoryService>();
        services.AddSingleton<ReportService>();
        services.AddSingleton<WalletService>();
        services.AddSingleton<SchedulerService>();

        services.AddSingleton<IFinanceService>(sp => new FinanceServiceLoggingDecorator(
            sp.GetRequiredService<FinanceService>(),
            sp.GetRequiredService<ILogger<FinanceServiceLoggingDecorator>>()));

        services.AddSingleton<ICategoryService>(sp => new CategoryServiceLoggingDecorator(
            sp.GetRequiredService<CategoryService>(),
            sp.GetRequiredService<ILogger<CategoryServiceLoggingDecorator>>()));

        services.AddSingleton<IReportService>(sp => new ReportServiceLoggingDecorator(
            sp.GetRequiredService<ReportService>(),
            sp.GetRequiredService<ILogger<ReportServiceLoggingDecorator>>()));

        services.AddSingleton<IWalletService>(sp => new WalletServiceLoggingDecorator(
            sp.GetRequiredService<WalletService>(),
            sp.GetRequiredService<ILogger<WalletServiceLoggingDecorator>>()));

        services.AddSingleton<ISchedulerService>(sp => new SchedulerServiceLoggingDecorator(
            sp.GetRequiredService<SchedulerService>(),
            sp.GetRequiredService<ILogger<SchedulerServiceLoggingDecorator>>()));

        return services;
    }
}
