using System.Data;
using System.Data.Common;
using Dapper;
using FluentResults;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Werter.FinTrackr.Shared.Models;

namespace Werter.FinTrackr.FinanceDaStorage;

public sealed class StockRepository(ILogger logger, string connectionString)
{
    public async Task<Result> InsertAsync(IEnumerable<StockDto> produtos, CancellationToken cancellationToken)
    {
        logger.LogInformation("[StockRepository] Iniciando a inserção dos dados no banco de dados");
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            foreach (var stock in produtos)
                await InsertStockAsync(stock, connection, transaction, cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError("[StockRepository] Erro ao inserir os dados no banco de dados");
            return Result.Fail("Erro ao inserir os dados no banco de dados");
        }

        logger.LogInformation("[StockRepository] Dados inseridos no banco de dados com sucesso");
        return Result.Ok();
    }

    private static async Task InsertStockAsync(StockDto stock, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken)
    {
        const string query = "INSERT INTO Stock (Value, Date) VALUES (@Value, @Date)";

        // Criando DynamicParameters
        var parameters = new DynamicParameters();
        parameters.Add("@Value", stock.Value, DbType.Decimal);
        parameters.Add("@Date", stock.DataAsDateTime, DbType.DateTime);

        var commandDefinition = new CommandDefinition(query, parameters, transaction: transaction, cancellationToken: cancellationToken);

        await connection.ExecuteAsync(commandDefinition);
    }
}