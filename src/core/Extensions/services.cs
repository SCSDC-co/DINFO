using dinfo.core.Handlers.Html;
using dinfo.core.Handlers.Json;
using dinfo.core.Handlers.Yaml;
using dinfo.core.Interfaces.Output;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace dinfo.core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDInfo(this IServiceCollection services, ServiceLifetime handlersLifetime = ServiceLifetime.Scoped)
    {
        switch (handlersLifetime)
        {
            case ServiceLifetime.Singleton:
                services.TryAddKeyedSingleton<IOutputHandler, JsonHandler>(".json");
                services.TryAddKeyedSingleton<IOutputHandler, HtmlHandler>(".html");
                services.TryAddKeyedSingleton<IOutputHandler, HtmlHandler>(".htm");
                services.TryAddKeyedSingleton<IOutputHandler, YamlHandler>(".yaml");
                break;
            case ServiceLifetime.Scoped:
                services.TryAddKeyedScoped<IOutputHandler, JsonHandler>(".json");
                services.TryAddKeyedScoped<IOutputHandler, HtmlHandler>(".html");
                services.TryAddKeyedScoped<IOutputHandler, HtmlHandler>(".htm");
                services.TryAddKeyedScoped<IOutputHandler, YamlHandler>(".yaml");
                break;
            case ServiceLifetime.Transient:
                services.TryAddKeyedTransient<IOutputHandler, JsonHandler>(".json");
                services.TryAddKeyedTransient<IOutputHandler, HtmlHandler>(".html");
                services.TryAddKeyedTransient<IOutputHandler, HtmlHandler>(".htm");
                services.TryAddKeyedTransient<IOutputHandler, YamlHandler>(".yaml");
                break;
        }

        return services;
    }
}
