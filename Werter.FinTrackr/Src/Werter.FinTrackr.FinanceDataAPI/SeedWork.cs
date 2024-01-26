using Werter.FinTrackr.FinanceDaStorage;

namespace Werter.FinTrackr.FinanceDataAPI;

public class SeedWork(ILogger<SeedWork> logger, InitialMigration initialMigration) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("[SeedWork] Iniciando a migração inicial");
        initialMigration.Execute(cancellationToken);

        logger.LogInformation("[SeedWork] Migração inicial concluída");
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}