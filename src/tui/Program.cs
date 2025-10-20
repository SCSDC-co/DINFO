using CliFx;
using dinfo.core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public static class Program
{
    public static async Task<int> Main()
    {
        var host = Host.CreateDefaultBuilder().ConfigureServices(ConfigureServices).Build();

        var application = new CliApplicationBuilder()
            .AddCommandsFromThisAssembly()
            .UseTypeActivator(host.Services.GetRequiredService)
            .SetExecutableName("dinfo")
            .Build();

        var result = await application.RunAsync();
        return result;
    }

    private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        services.AddSingleton<DinfoCommand>();
        services.AddSingleton<FileCommand>();

        services.AddDInfo(ServiceLifetime.Singleton);
    }
}
