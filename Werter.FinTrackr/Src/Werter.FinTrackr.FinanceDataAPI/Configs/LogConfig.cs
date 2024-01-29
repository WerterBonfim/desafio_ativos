using Serilog;

namespace Werter.FinTrackr.FinanceDataAPI.Configs;

public static class LogConfig
{
    public static void AddLogConfig(this IServiceCollection services, WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();

        builder.Host.UseSerilog();

        var log = new LoggerConfiguration();

        log = log.WriteTo.Console();
        
        if (builder.Environment.IsProduction())
            log = log.Enrich.FromLogContext()
                     .WriteTo.File("/var/log/app/app.log", rollingInterval: RollingInterval.Day);

        // Configuração do Serilog
        Log.Logger = log.CreateLogger();
        builder.Logging.AddSerilog(Log.Logger);
    }
}