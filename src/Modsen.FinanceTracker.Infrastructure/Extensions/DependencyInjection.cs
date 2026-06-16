using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Modsen.FinanceTracker.BLL.Decorators;
using Modsen.FinanceTracker.BLL.Interfaces;
using Modsen.FinanceTracker.Infrastructure.Services;

namespace Modsen.FinanceTracker.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddHttpClient<CurrencyService>();
        services.AddSingleton<ICurrencyService>(sp => new CachedCurrencyService(
                sp.GetRequiredService<CurrencyService>(),
                sp.GetRequiredService<IMemoryCache>()));

        return services;
    }
}
