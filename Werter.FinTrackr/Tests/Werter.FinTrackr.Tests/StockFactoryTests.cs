using Werter.FinTrackr.FinanceDataAPI.Factories;
using Werter.FinTrackr.Shared.Models;

namespace Werter.FinTrackr.Tests;

public class StockFactoryTests
{
    private readonly StockFactory _stockFactory;
    
    public StockFactoryTests()
    {
        _stockFactory = new StockFactory();
    }
    
    
    [Fact(DisplayName = "ExecuteAsync should return StocksWithVariation list on success")]
    [Trait("Category", "StockFactory")]
    public void ExecuteAsync_ReturnsStocksWithVariationList_OnSuccess()
    {
        // Arrange

        var fakeStocks = new List<Stock>
        {
            new(1, "aaa", 3.0m, new DateTime(2024, 1, 26)),
            new(2, "bbb", 2.0m, new DateTime(2024, 1, 25)),
            new(3, "ccc", 1.0m, new DateTime(2024, 1, 24)),
        };

        var expectedResult = Result.Ok(new List<StocksWithVariation>
        {
            new(24, new DateOnly(2024, 1, 24), 1.0m, 0, 0.0m),
            new(25, new DateOnly(2024, 1, 25), 2.0m, 1.0m, 1.0m),
            new(26, new DateOnly(2024, 1, 26), 3.0m, 1.0m, 2.0m),
        });
        

        // Act
        var stocksWithVariations = _stockFactory.CalculateVariations(fakeStocks);

        // Assert
        stocksWithVariations.Should().BeEquivalentTo(expectedResult.Value);
    }
}