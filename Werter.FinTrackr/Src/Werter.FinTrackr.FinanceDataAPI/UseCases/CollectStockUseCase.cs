using FluentResults;
using Werter.FinTrackr.FinanceDaStorage;
using Werter.FinTrackr.FinanceDataCollector;

namespace Werter.FinTrackr.FinanceDataAPI.UseCases;

public sealed class CollectStockUseCase(
    ILogger<CollectStockUseCase> logger,
    FinanceDataService financeDataService,
    StockRepository stockRepository
) : IUseCase<string, Result>
{
    public async Task<Result> ExecuteAsync(string input, CancellationToken cancellationToken)
    {
        logger.LogInformation("Realizando a coleta dos dados do ativo {StockName}", input);

        var collectResult = await financeDataService.CollectDataAsync(input, cancellationToken);

        if (collectResult.IsFailed)
            return Result.Fail(collectResult.Errors);

        var stocks = collectResult.Value;

        var insertResult = await stockRepository.InsertAsync(stocks, cancellationToken);
        
        return insertResult;
    }
}