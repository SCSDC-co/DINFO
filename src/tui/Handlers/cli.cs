using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using dinfo.core.Interfaces.Output;
using dinfo.core.Utils.Globals;
using dinfo.tui.Helpers;
using dinfo.tui.Helpers.Tui;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

[Command(Description = "Display information about the specified directory and its contents.")]
public class DinfoCommand(IServiceProvider serviceProvider, TuiHelper tuiHelper) : ICommand
{
    [CommandParameter(0, Description = "The Directory to be analyzed.", IsRequired = false)]
    public string TargetDirectory { get; set; } = Environment.CurrentDirectory;

    [CommandOption("recursive", 'r', Description = "Recursively list all files and directories.")]
    public bool RecursiveCli { get; set; } = false;

    [CommandOption("verbose", 'v', Description = "Enable verbose output.")]
    public bool VerboseCli { get; set; } = false;

    [CommandOption("ignore-gitignore", 'i', Description = "Ignore .gitignore files.")]
    public bool IgnoreGitIgnoreCli { get; set; } = false;

    [CommandOption("output", 'o', Description = "Specify if you want the output in a file.")]
    public bool OutputToFileCli { get; set; } = false;

    [CommandOption("file-format",
        'f',
        Description = "Specify the output file (formats: json, yaml)."
    )]
    public string OutputCli { get; set; } = "output.json";

    [CommandOption("no-tui", 'n', Description = "Disable TUI")]
    public bool NoTuiCli { get; set; } = false;

    public async ValueTask ExecuteAsync(IConsole console)
    {
        var outputCli = Path.GetExtension(OutputCli).ToLowerInvariant();
        var handler = serviceProvider.GetRequiredKeyedService<IOutputHandler>(outputCli);

        GlobalsUtils.Recursive = RecursiveCli;
        GlobalsUtils.Verbose = VerboseCli;
        GlobalsUtils.IgnoreGitignore = IgnoreGitIgnoreCli;
        GlobalsUtils.NoTui = NoTuiCli;

        var dir = TargetDirectory;

        if (!GlobalsUtils.NoTui)
        {
            await tuiHelper.PrintDirectoryInfoAsync(dir);
        }

        if (OutputToFileCli)
        {
            await handler.DirectorySaveAsync(dir, OutputCli);
            AnsiConsoleHelper.Print(OutputCli);
        }
        else
        {
            AnsiConsole.MarkupLine("[bold green]Output will not be saved in a file[/]");
        }
    }
}

[Command("file", Description = "Display information about the specified file.")]
public class FileCommand(IServiceProvider serviceProvider, TuiHelper tuiHelper) : ICommand
{
    [CommandParameter(0, Description = "The File to be analyzed.", IsRequired = true)]
    public required string TargetFile { get; set; }

    [CommandOption("output", 'o', Description = "Specify if you want the output in a file.")]
    public bool OutputToFileCli { get; set; } = false;

    [CommandOption(
        "file-format",
        'f',
        Description = "Specify the output file (formats: json, yaml, html)."
    )]
    public string OutputCli { get; set; } = "output.json";

    [CommandOption("no-tui", 'n', Description = "Disable TUI")]
    public bool NoTuiCli { get; set; } = false;

    public async ValueTask ExecuteAsync(IConsole console)
    {
        var outputCli = Path.GetExtension(OutputCli).ToLowerInvariant();
        var handler = serviceProvider.GetRequiredKeyedService<IOutputHandler>(outputCli);

        GlobalsUtils.NoTui = NoTuiCli;

        if (!GlobalsUtils.NoTui)
        {
            await tuiHelper.PrintFileInfoAsync(TargetFile);
        }

        if (OutputToFileCli)
        {
            await handler.FileSaveAsync(TargetFile, OutputCli);
            AnsiConsoleHelper.Print(OutputCli);
        }
        else
        {
            AnsiConsole.MarkupLine("[bold green]Output will not be saved in a file[/]");
        }
    }
}
