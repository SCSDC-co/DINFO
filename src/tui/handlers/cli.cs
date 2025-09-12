using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using dinfo.core.Handlers.Json;
using dinfo.core.Utils.Globals;
using dinfo.tui.Helpers.Tui;
using Spectre.Console;

[Command(Description = "Display information about the specified directory and its contents.")]
public class DinfoCommand : ICommand
{
    [CommandParameter(0, Description = "The Directory to be analyzed.", IsRequired = false)]
    public string TargetDirectory { get; set; } = Directory.GetCurrentDirectory();

    [CommandOption("recursive", 'r', Description = "Recursively list all files and directories.")]
    public bool RecursiveCli { get; set; } = false;

    [CommandOption("verbose", 'v', Description = "Enable verbose output.")]
    public bool VerboseCli { get; set; } = false;

    [CommandOption("ignore-gitignore", 'i', Description = "Ignore .gitignore files.")]
    public bool IgnoreGitIgnoreCli { get; set; } = false;

    [CommandOption("output", 'o', Description = "Specify if and where you want to save the output in a file (formats: json).")]
    public string OutputCli { get; set; } = "output.json";

    [CommandOption("no-tui", 'n', Description = "Disable TUI")]
    public bool NoTuiCli { get; set; } = false;

    public async ValueTask ExecuteAsync(IConsole console)
    {
        GlobalsUtils.Recursive = RecursiveCli;
        GlobalsUtils.Verbose = VerboseCli;
        GlobalsUtils.IgnoreGitignore = IgnoreGitIgnoreCli;
        GlobalsUtils.NoTui = NoTuiCli;

        var dir = TargetDirectory;

        if (!GlobalsUtils.NoTui)
        {
            await TuiHelper.PrintDirectoryInfo(dir);
        }

        if (OutputCli.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
        {
            await JsonHandler.DirectorySaveJson(dir, OutputCli);

            var savedPanel = new Panel($"[bold green]JSON file saved in:[/] {OutputCli}");
            savedPanel.Border = BoxBorder.Rounded;
            savedPanel.BorderStyle = new Style(Color.Green);
            AnsiConsole.Write(savedPanel);
        }
        else
        {
            AnsiConsole.MarkupLine("[bold red]The output must be a .json file[/]");
        }
    }
}

[Command("file", Description = "Display information about the specified file.")]
public class FileCommand : ICommand
{
    [CommandParameter(0, Description = "The File to be analyzed.", IsRequired = true)]
    public required string TargetFile { get; set; }

    [CommandOption("output", 'o', Description = "Output format and name.")]
    public string OutputCli { get; set; } = "output.json";

    [CommandOption("no-tui", 'n', Description = "Disable TUI")]
    public bool NoTuiCli { get; set; } = false;

    public async ValueTask ExecuteAsync(IConsole console)
    {
        GlobalsUtils.NoTui = NoTuiCli;

        if (!GlobalsUtils.NoTui)
        {
            await TuiHelper.PrintFileInfo(TargetFile);
        }

        if (OutputCli.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
        {
            await JsonHandler.FileSaveJson(TargetFile, OutputCli);

            var savedPanel = new Panel($"[bold green]JSON file saved in:[/] {OutputCli}");
            savedPanel.Border = BoxBorder.Rounded;
            savedPanel.BorderStyle = new Style(Color.Green);
            AnsiConsole.Write(savedPanel);
        }
        else
        {
            AnsiConsole.MarkupLine("[bold red]The output must be a .json file[/]");
        }
    }
}
