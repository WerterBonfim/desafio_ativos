using Microsoft.AspNetCore.Mvc;
using Werter.FinTrackr.FinanceDataAPI.Configs;
using Werter.FinTrackr.FinanceDataAPI.UseCases;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiConfiguration(builder);

var app = builder.Build();


app.MapGet("/", () => "Hello World!");
app.MapGet("/collect-stock", async (
               [FromServices] CollectStockUseCase useCase,
               [FromQuery] string stock = "PETR4.SA",
               CancellationToken cancellationToken = default) =>
           {
               var result = await useCase.ExecuteAsync(stock, cancellationToken);
               return Results.Ok(result);
           });

app.UseApiConfiguration(app.Environment);

app.Run();