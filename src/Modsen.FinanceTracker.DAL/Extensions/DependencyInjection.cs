using Microsoft.Extensions.DependencyInjection;
using Modsen.FinanceTracker.DAL.Context;
using Modsen.FinanceTracker.DAL.Repositories;
using Modsen.FinanceTracker.Domain.Interfaces;

namespace Modsen.FinanceTracker.DAL.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddDAL(this IServiceCollection services, string jsonDbPath)
    {
        services.AddSingleton(sp => new JsonDbContext(jsonDbPath));
        services.AddSingleton<IDataContext>(sp => sp.GetRequiredService<JsonDbContext>());

        services.AddSingleton<ICategoryRepository, JsonCategoryRepository>();
        services.AddSingleton<IWalletRepository, JsonWalletRepository>();
        services.AddSingleton<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
