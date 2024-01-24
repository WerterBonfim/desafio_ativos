using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Werter.FinTrackr.Shared;

public static class DependencyInjection
{
    public static void AddShared(this IServiceCollection services, ConfigurationManager configuration)
    {
        var appConfig = configuration
                       .GetSection("AppConfig")
                       .Get<AppConfig>() ?? throw new InvalidOperationException(
            "Erro ao obter a configuração AppConfig no appsettings/env");
        
        if (appConfig.LogDiretory == null)
            throw new InvalidOperationException(
                "Erro ao obter a configuração DiretorioDeLog no appsettings/env");
        
        
        
    }
}