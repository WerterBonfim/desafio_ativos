using FluentResults;
using Werter.FinTrackr.FinanceDaStorage;
using Werter.FinTrackr.Shared.Models;

namespace Werter.FinTrackr.FinanceDataAPI.UseCases;

public sealed class StockUseCase : IUseCase<string, Result<List<StocksWithVariation>>>
{
    private readonly ILogger<StockUseCase> _logger;
    private readonly StockRepository _stockRepository;

    public StockUseCase(ILogger<StockUseCase> logger, StockRepository stockRepository)
    {
        _logger = logger;
        _stockRepository = stockRepository;
    }

    public async Task<Result<List<StocksWithVariation>>> ExecuteAsync(string input, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[AssetsUseCase] Realizando a busca de ativos");
        
        var stocks = await _stockRepository.ListAsync(input, cancellationToken); 
        if (stocks.IsFailed)
            return Result.Fail<List<StocksWithVariation>>(stocks.Errors);
        
        var stocksWithVariation = StockHelper.CalculateVariations(stocks.Value);
        
        return Result.Ok(stocksWithVariation);
    }

    private static class StockHelper
    {
        public static List<StocksWithVariation> CalculateVariations(List<Stock> stocks)
        {
            if (stocks.Count == 0)
                return Enumerable.Empty<StocksWithVariation>().ToList();

            var sortedStocks = stocks.OrderBy(a => a.Date).ToList();
            var firstStock = sortedStocks.First();

            var resultado = sortedStocks.Select((acao, index) =>
            {
                var variationFromPreviousDay = index > 0 ? acao.Value - sortedStocks[index - 1].Value : 0m;
                var variationFromFirstDate = acao.Value - firstStock.Value;

                return new StocksWithVariation(index + 1, acao.Date, acao.Value, variationFromPreviousDay, variationFromFirstDate);
                
            }).ToList();

            return resultado;
        }
    }
}