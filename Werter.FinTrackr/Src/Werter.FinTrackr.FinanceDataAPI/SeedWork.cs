using Werter.FinTrackr.FinanceDaStorage;

namespace Werter.FinTrackr.FinanceDataAPI;

public class SeedWork(ILogger<SeedWork> logger, InitialMigration initialMigration) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("[SeedWork] Iniciando a migração inicial");
        await initialMigration.Execute(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}