using Werter.FinTrackr.Shared;
using Werter.FinTrackr.Shared.Models;

namespace Werter.FinTrackr.FinanceDataAPI.Factories;

public interface IStockFactory
{
    List<StocksWithVariation> CalculateVariations(IReadOnlyCollection<Stock> stocks);
}

public sealed class StockFactory : IStockFactory
{
    public List<StocksWithVariation> CalculateVariations(IReadOnlyCollection<Stock> stocks)
    {
        if (stocks.Count == 0)
            return Enumerable.Empty<StocksWithVariation>().ToList();

        var sortedStocks = stocks.OrderBy(a => a.Date).ToList();
        var firstStock = sortedStocks.First();

        var resultado = sortedStocks.Select((acao, index) =>
        {
            var variationFromPreviousDay = index > 0 ? acao.Value - sortedStocks[index - 1].Value : 0m;
            var variationFromFirstDate = acao.Value - firstStock.Value;

            return new StocksWithVariation(
                acao.Date.Day,
                new DateOnly(acao.Date.Year, acao.Date.Month, acao.Date.Day)  ,
                acao.Value.RoundMonetaryValue(),
                variationFromPreviousDay,
                variationFromFirstDate
            );
        }).ToList();

        return resultado;
    }
}