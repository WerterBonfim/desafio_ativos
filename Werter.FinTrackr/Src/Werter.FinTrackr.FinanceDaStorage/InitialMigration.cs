using Dapper;
using FluentResults;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Werter.FinTrackr.Shared;

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

    private static async Task ChangeDatabaseAsync(SqlConnection connection, string databaseName, CancellationToken cancellationToken) 
        => await connection.ExecuteAsync(new CommandDefinition("USE " + databaseName, cancellationToken: cancellationToken));

    private static async Task CreateObjectsIfNotExisteAsync(SqlConnection connection, CancellationToken cancellationToken)
    {
        const string query = "IF NOT EXISTS (SELECT [name] FROM sys.databases WHERE [name] = N'DB_FinTrackr') CREATE DATABASE DB_FinTrackr";
        var commandDefinition = new CommandDefinition(query, cancellationToken: cancellationToken);
        await connection.ExecuteAsync(commandDefinition);

        await ChangeDatabaseAsync(connection, Constants.DbName, cancellationToken);

        // Verificar se a tabela existe e criá-la se não existir
        const string createTableIfNotExists = """
                                                          IF OBJECT_ID('[dbo].[Stock]', 'U') IS NULL
                                                              CREATE TABLE [dbo].[Stock]
                                                              (
                                                                  [Id]    INT             NOT NULL IDENTITY (1,1) PRIMARY KEY,
                                                                  [Name]  VARCHAR(50)     NOT NULL,
                                                                  [Value] DECIMAL(18, 14),
                                                                  [Date]  DATETIME        NOT NULL
                                                              )
                                              """;
        
        var createTableIfNotExistsCommand = new CommandDefinition(createTableIfNotExists, cancellationToken: cancellationToken);
        await connection.ExecuteAsync(createTableIfNotExistsCommand);

    }


}