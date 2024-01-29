using FluentResults;
using Werter.FinTrackr.FinanceDaStorage;
using Werter.FinTrackr.FinanceDataAPI.Factories;
using Werter.FinTrackr.Shared.Models;

namespace Werter.FinTrackr.FinanceDataAPI.UseCases;

public sealed class StockUseCase : IUseCase<string, Result<List<StocksWithVariation>>>
{
    private readonly ILogger<StockUseCase> _logger;
    private readonly IStockRepository _stockRepository;
    private readonly IStockFactory _stockFactory;

    public StockUseCase(ILogger<StockUseCase> logger, IStockRepository stockRepository, IStockFactory stockFactory)
    {
        _logger = logger;
        _stockRepository = stockRepository;
        _stockFactory = stockFactory;
    }

    public async Task<Result<List<StocksWithVariation>>> ExecuteAsync(string input, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[AssetsUseCase] Realizando a busca de ativos");

        var stocks = await _stockRepository.ListAsync(input, cancellationToken);
        if (stocks.IsFailed)
            return Result.Fail<List<StocksWithVariation>>(stocks.Errors);

        var stocksWithVariation = _stockFactory.CalculateVariations(stocks.Value);

        return Result.Ok(stocksWithVariation);
    }
    
}