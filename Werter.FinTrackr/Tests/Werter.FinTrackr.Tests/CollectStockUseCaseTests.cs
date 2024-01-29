using Werter.FinTrackr.FinanceDaStorage;
using Werter.FinTrackr.FinanceDataAPI.UseCases;
using Werter.FinTrackr.FinanceDataCollector;
using Werter.FinTrackr.Shared.Models;

namespace Werter.FinTrackr.Tests;

public class CollectStockUseCaseTests
{
    private readonly CollectStockUseCase _collectStockUseCase;
    private readonly IFinanceDataService _financeDataService;
    private readonly IStockRepository _stockRepository;

    public CollectStockUseCaseTests()
    {
        _financeDataService = A.Fake<IFinanceDataService>();
        _stockRepository = A.Fake<IStockRepository>();
        var logger = A.Fake<ILogger<CollectStockUseCase>>();
        
        _collectStockUseCase = new CollectStockUseCase(logger, _financeDataService, _stockRepository);
    }

    [Fact(DisplayName = "ExecuteAsync should return StocksWithVariation list on success")]
    [Trait("Category", "CollectStockUseCase")]
    public async Task ExecuteAsync_SuccessfullyCollectsAndInsertsStockData()
    {
        // Arrange
        var input = "acao1";
        var cancellationToken = CancellationToken.None;
        var fakeStockData = new List<StockDto>
        {
            new(input, 10d, DateTimeOffset.Now.ToUnixTimeSeconds()),
            new(input, 20d, DateTimeOffset.Now.ToUnixTimeSeconds()),
            new(input, 30d, DateTimeOffset.Now.ToUnixTimeSeconds()),
        };

        A.CallTo(
            () => _financeDataService.CollectDataAsync(An<string>._, An<CancellationToken>._)
        ).Returns(fakeStockData);

        A.CallTo(
            () => _stockRepository.TruncateAsync(An<CancellationToken>._)
        ).Returns(Result.Ok());

        A.CallTo(
            () => _stockRepository.InsertAsync(An<IEnumerable<StockDto>>._, An<CancellationToken>._)
        ).Returns(Result.Ok());
        

        // Act
        var result = await _collectStockUseCase.ExecuteAsync(input, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
       
        A.CallTo(
            () => _stockRepository.TruncateAsync(An<CancellationToken>._)
        ).MustHaveHappenedOnceExactly();
        
        A.CallTo(
            () => _stockRepository.InsertAsync(An<IEnumerable<StockDto>>._, An<CancellationToken>._)
        ).MustHaveHappenedOnceExactly();
        
    }

    [Fact(DisplayName = "ExecuteAsync should return failure when data collection fails")]
    [Trait("Category", "CollectStockUseCase")]
    public async Task ExecuteAsync_ReturnsFailure_WhenDataCollectionFails()
    {
        // Arrange
        var input = "acao1";
        var cancellationToken = CancellationToken.None;
        var fakeError = new Error("Nao foi possivel coletar os dados");
        
        A.CallTo(
            () => _financeDataService.CollectDataAsync(An<string>._, An<CancellationToken>._)
        ).Returns(Result.Fail<List<StockDto>>(fakeError));
        
    
        // Act
        var result = await _collectStockUseCase.ExecuteAsync(input, cancellationToken);
    
        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(fakeError, result.Errors);
        
        A.CallTo(
            () => _stockRepository.TruncateAsync(An<CancellationToken>._)
        ).MustNotHaveHappened();
        
        A.CallTo(
            () => _stockRepository.InsertAsync(An<IEnumerable<StockDto>>._, An<CancellationToken>._)
        ).MustNotHaveHappened();
        
    }
}