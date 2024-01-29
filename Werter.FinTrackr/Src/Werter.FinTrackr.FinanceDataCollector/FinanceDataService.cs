using System.Net.Http.Json;
using FluentResults;
using Microsoft.Extensions.Logging;
using Werter.FinTrackr.Shared.Models;

namespace Werter.FinTrackr.FinanceDataCollector;

public interface IFinanceDataService
{
    Task<Result<List<StockDto>>> CollectDataAsync(string stockName, CancellationToken cancellationToken);
}

public class FinanceDataService(ILogger<FinanceDataService> logger, HttpClient httpClient) : IFinanceDataService
{
    private const string Url = "https://query2.finance.yahoo.com/v8/finance/chart/{0}?period1={1}&period2={2}&interval=1d";

    private static string GetUrl(string stockName)
    {
        var first = new DateTimeOffset(DateTime.Now.Subtract(TimeSpan.FromDays(50))).ToUnixTimeSeconds();
        var last = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
        
        return string.Format(Url, stockName, first, last);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stockName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Result<List<StockDto>>> CollectDataAsync(string stockName, CancellationToken cancellationToken)
    {
        try
        {
            var url = GetUrl(stockName);
            var response = await httpClient.GetFromJsonAsync<RequestResultApi>(url, cancellationToken: cancellationToken);

            if (response is not { Erro: null })
            {
                logger.LogError("Erro ao obter os dados da API yahoo finance: {Erro}", response?.Erro);
                return Result.Fail("Erro ao obter os dados da API yahoo finance");
            }

            var result = response.Chart.Result.FirstOrDefault();

            if (result == null)
            {
                logger.LogError("A Api do yahoo finance retornou um resultado vazio no objeto chart.result");
                return Result.Fail("A Api do yahoo finance retornou um resultado vazio no objeto");
            }

            var last30Stocks = result.Indicators.Quote[0].Open
                                     .Select((value, index) => new StockDto(stockName, value, result.Timestamp[index]))
                                     .OrderByDescending(x => x.Date)
                                     .Take(30)
                                     .ToList();

            return Result.Ok(last30Stocks);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Erro ao obter os dados da API yahoo finance");
            return Result.Fail("Erro ao obter os dados da API yahoo finance");
        }
    }
}
