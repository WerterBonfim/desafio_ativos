using FluentResults;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Werter.FinTrackr.FinanceDaStorage;

public class InitialMigration
{
    private readonly ILogger<InitialMigration> _logger;
    private readonly string _connectionString;

    public InitialMigration(ILogger<InitialMigration> logger, string connectionString)
    {
        _logger = logger;
        _connectionString = connectionString;
    }


    public async Task<Result> Execute(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("[InitialMigration] Verificar se existe a migração inicial");

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await CreateObjectsIfNotExisteAsync(connection, cancellationToken);
            
            return Result.Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[InitialMigration] Erro ao executar a migração");
            return Result.Fail("Erro ao executar a migração");
        }
    }

    private static async Task CreateObjectsIfNotExisteAsync(SqlConnection connection, CancellationToken cancellationToken)
    {
        const string query = "IF NOT EXISTS (SELECT [name] FROM sys.databases WHERE [name] = N'DB_FinTrackr') CREATE DATABASE DB_FinTrackr";
        var command = new SqlCommand(query, connection);
        await command.ExecuteNonQueryAsync(cancellationToken);

        // Agora que o banco de dados foi criado, você pode abrir uma nova conexão para ele
        connection.ChangeDatabase("DB_FinTrackr");

        // Verificar se a tabela existe e criá-la se não existir
        const string createTableIfNotExists = @"
            IF OBJECT_ID('[dbo].[Stock]', 'U') IS NULL
                CREATE TABLE [dbo].[Stock]
                (
                    [Id]    INT             NOT NULL IDENTITY (1,1) PRIMARY KEY,
                    [Value] DECIMAL(18, 14),
                    [Date]  DATETIME        NOT NULL
                )";

        await using var commandCreateTable = new SqlCommand(createTableIfNotExists, connection);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }


}