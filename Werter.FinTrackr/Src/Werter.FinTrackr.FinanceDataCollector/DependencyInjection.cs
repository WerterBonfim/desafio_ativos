using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Werter.FinTrackr.FinanceDataCollector;

public static class DependencyInjection
{
    public static void AddFinanceDataCollector(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddHttpClient();
        services.AddScoped<FinanceDataService>();
    }
}