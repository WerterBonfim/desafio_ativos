using Werter.FinTrackr.FinanceDaStorage;
using Werter.FinTrackr.FinanceDataAPI.Factories;
using Werter.FinTrackr.FinanceDataAPI.UseCases;
using Werter.FinTrackr.FinanceDataCollector;

namespace Werter.FinTrackr.FinanceDataAPI.Configs;

public static class ApiConfig
{
    public static void AddApiConfiguration(this IServiceCollection services, WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;

        services.AddLogConfig(builder);

        services.AddHealthChecks();
        
        services.AddFinanceDaStorage(configuration);
        services.AddFinanceDataCollector(configuration);

        services.AddHostedService<SeedWork>();

        services.AddScoped<IStockFactory, StockFactory>();
        services.AddScoped<CollectStockUseCase>();

        services.AddScoped<StockUseCase>();

    }

    public static void UseApiConfiguration(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (!env.IsDevelopment()) return;
        app.UseDeveloperExceptionPage();

    }
    
}