using System.Net.Http.Json;
using FluentResults;
using Microsoft.Extensions.Logging;
using Werter.FinTrackr.Shared.Models;

namespace Werter.FinTrackr.FinanceDataCollector;

public sealed class FinanceDataService(ILogger<FinanceDataService> logger, HttpClient httpClient)
{
    private const string Url = "https://query1.finance.yahoo.com/v8/finance/chart/{0}";

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
            var url = string.Format(Url, stockName);
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
                                     .Take(30)
                                     .Select((value, index) => new StockDto(stockName, value, result.Timestamp[index]))
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
