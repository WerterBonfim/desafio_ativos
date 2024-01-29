using Werter.FinTrackr.FinanceDaStorage;
using Werter.FinTrackr.FinanceDataAPI.Factories;
using Werter.FinTrackr.FinanceDataAPI.UseCases;
using Werter.FinTrackr.Shared.Models;

namespace Werter.FinTrackr.Tests;

public class StockUseCaseTests
{
    private readonly IStockRepository _stockRepository;
    private readonly IStockFactory _stockFactory;
    private readonly StockUseCase _stockUseCase;

    public StockUseCaseTests()
    {
        _stockRepository = A.Fake<IStockRepository>();
        _stockFactory = A.Fake<IStockFactory>();
        _stockUseCase = new StockUseCase(A.Fake<ILogger<StockUseCase>>(), _stockRepository, _stockFactory);
        
    }

    // Teste somente os fluxos de sucesso
    [Fact(DisplayName = "ExecuteAsync should return StocksWithVariation list on success")]
    [Trait("Category", "StockUseCase")]
    public async Task ExecuteAsync_ReturnsStocksWithVariationList_OnSuccess()
    {
        // Arrange

        var input = "testInput";
        var cancellationToken = CancellationToken.None;
       

        A.CallTo(
            () => _stockRepository.ListAsync(An<string>._, An<CancellationToken>._)
        ).Returns( new List<Stock>());
        
        A.CallTo(
            () => _stockFactory.CalculateVariations(An<IReadOnlyCollection<Stock>>._)
        ).Returns([]);
        

        // Act
        var result = await _stockUseCase.ExecuteAsync(input, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        A.CallTo(
            () => _stockRepository.ListAsync(An<string>._, An<CancellationToken>._)
        ).MustHaveHappenedOnceExactly();
        
        A.CallTo(
            () => _stockFactory.CalculateVariations(An<IReadOnlyCollection<Stock>>._)
        ).MustHaveHappenedOnceExactly();

        
    }
    
    [Fact(DisplayName = "ExecuteAsync should return an empty StocksWithVariation list when repository returns empty list")]
    [Trait("Category", "StockUseCase")]
    public async Task ExecuteAsync_ReturnsEmptyStocksWithVariationList_WhenRepositoryReturnsEmptyList()
    {
        // Arrange

        var input = "testInput";
        var cancellationToken = CancellationToken.None;

        var expectedResult = Result.Ok(new List<StocksWithVariation>());
        

        A.CallTo(
            () => _stockRepository.ListAsync(An<string>._, An<CancellationToken>._)
        ).Returns(new List<Stock>());
        
        // Act
        var result = await _stockUseCase.ExecuteAsync(input, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();

        result.Value.Should().BeEquivalentTo(expectedResult.Value);
    }

    [Fact(DisplayName = "ExecuteAsync should return failure when repository fails")]
    [Trait("Category", "StockUseCase")]
    public async Task ExecuteAsync_ReturnsFailure_WhenRepositoryFails()
    {
        // Arrange
        var input = "testInput";
        var cancellationToken = CancellationToken.None;
        var fakeError = new Error("Repository failure");
        
        A.CallTo(
            () => _stockRepository.ListAsync(An<string>._, An<CancellationToken>._)
        ).Returns(Result.Fail<List<Stock>>(fakeError));
        

        // Act
        var result = await _stockUseCase.ExecuteAsync(input, cancellationToken);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(fakeError);
    }
}