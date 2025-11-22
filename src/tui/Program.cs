using CliFx;
using dinfo.core.Extensions;
using dinfo.tui.Helpers.Tui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class Program
{
    public static async Task<int> Main()
    {
        var host = Host.CreateDefaultBuilder().ConfigureServices(ConfigureServices).Build();

        var application = new CliApplicationBuilder()
            .AddCommandsFromThisAssembly()
            .UseTypeActivator(host.Services.GetRequiredService)
            .SetExecutableName("dinfo")
            .Build();

        var logger = host.Services.GetRequiredService<ILogger<Program>>();
        var result = await application.RunAsync();

        logger.LogDebug("Program exited with code {result}", result);
        return result;
    }

    private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        services.AddSingleton<DinfoCommand>();
        services.AddSingleton<FileCommand>();
        services.AddSingleton<TuiHelper>();

        services.AddDInfo(ServiceLifetime.Singleton);
    }
}
