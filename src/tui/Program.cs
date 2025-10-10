using CliFx;

public static class Program
{
    public static async Task<int> Main() =>
        await new CliApplicationBuilder().AddCommandsFromThisAssembly().SetExecutableName("dinfo").Build().RunAsync();
}
