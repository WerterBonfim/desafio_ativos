using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Werter.FinTrackr.FinanceDaStorage;

public static class DependencyInjection
{
    public static void AddFinanceDaStorage(this IServiceCollection services, ConfigurationManager configuration)
    {
        var finTrackrConnection = configuration.GetConnectionString("FinTrackr")
                               ?? throw new InvalidOperationException("Nao foi possivel encontrar a string de Conexão (FinTrackr) atraves de appsettings/env");

        var initalMigrationConnection = configuration.GetConnectionString("InitialMigration")
                                     ?? throw new InvalidOperationException("Nao foi possivel encontrar a string de Conexão (InitialMigration) atraves de appsettings/env");


        services.AddScoped<StockRepository>(x =>
        {
            var logger = x.GetRequiredService<ILogger<StockRepository>>();
            return new StockRepository(logger, finTrackrConnection);
        });

        services.AddSingleton<InitialMigration>(x =>
        {
            var logger = x.GetRequiredService<ILogger<InitialMigration>>();
            return new InitialMigration(logger, initalMigrationConnection);
        });
    }
}