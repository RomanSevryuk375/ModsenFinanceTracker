namespace Modsen.FinanceTracker.DAL.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddDAL(this IServiceCollection services, string jsonDbPath)
    {
        services.AddSingleton(sp => new JsonDbContext(
            jsonDbPath,
            sp.GetRequiredService<ILogger<JsonDbContext>>()));
        services.AddSingleton<IDataContext>(sp => sp.GetRequiredService<JsonDbContext>());

        services.AddSingleton<ICategoryRepository, JsonCategoryRepository>();
        services.AddSingleton<IWalletRepository, JsonWalletRepository>();
        services.AddSingleton<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
