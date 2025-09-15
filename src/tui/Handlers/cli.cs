using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using dinfo.core.Handlers.Json;
using dinfo.core.Handlers.Yaml;
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

    [CommandOption("output", 'o', Description = "Specify if you want the output in a file.")]
    public bool OutputToFileCli { get; set; } = false;

    [CommandOption(
        "file-format",
        'f',
        Description = "Specify the output file (formats: json, yaml)."
    )]
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
            await TuiHelper.PrintDirectoryInfoAsync(dir);
        }

        if (OutputToFileCli)
        {
            if (OutputCli.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                await JsonHandler.DirectorySaveJsonAsync(dir, OutputCli);

                var savedPanel = new Panel($"[bold green]JSON file saved in:[/] {OutputCli}");

                savedPanel.Border = BoxBorder.Rounded;
                savedPanel.BorderStyle = new Style(Color.Green);

                AnsiConsole.Write(savedPanel);
            }
            else if (OutputCli.EndsWith(".yaml", StringComparison.OrdinalIgnoreCase))
            {
                await YamlHandler.DirectorySaveYamlAsync(dir, OutputCli);

                var savedPanel = new Panel($"[bold green]YAML file saved in:[/] {OutputCli}");

                savedPanel.Border = BoxBorder.Rounded;
                savedPanel.BorderStyle = new Style(Color.Green);

                AnsiConsole.Write(savedPanel);
            }
            else
            {
                AnsiConsole.MarkupLine("[bold red]The output must be a .json/.yaml file[/]");
            }
        }
        else
        {
            AnsiConsole.MarkupLine("[bold green]Output will not be saved in a file[/]");
        }
    }
}

[Command("file", Description = "Display information about the specified file.")]
public class FileCommand : ICommand
{
    [CommandParameter(0, Description = "The File to be analyzed.", IsRequired = true)]
    public required string TargetFile { get; set; }

    [CommandOption("output", 'o', Description = "Specify if you want the output in a file.")]
    public bool OutputToFileCli { get; set; } = false;

    [CommandOption(
        "file-format",
        'f',
        Description = "Specify the output file (formats: json, yaml)."
    )]
    public string OutputCli { get; set; } = "output.json";

    [CommandOption("no-tui", 'n', Description = "Disable TUI")]
    public bool NoTuiCli { get; set; } = false;

    public async ValueTask ExecuteAsync(IConsole console)
    {
        GlobalsUtils.NoTui = NoTuiCli;

        if (!GlobalsUtils.NoTui)
        {
            await TuiHelper.PrintFileInfoAsync(TargetFile);
        }

        if (OutputToFileCli)
        {
            if (OutputCli.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                await JsonHandler.FileSaveJsonAsync(TargetFile, OutputCli);

                var savedPanel = new Panel($"[bold green]JSON file saved in:[/] {OutputCli}");

                savedPanel.Border = BoxBorder.Rounded;
                savedPanel.BorderStyle = new Style(Color.Green);

                AnsiConsole.Write(savedPanel);
            }
            else if (OutputCli.EndsWith(".yaml", StringComparison.OrdinalIgnoreCase))
            {
                await YamlHandler.FileSaveYamlAsync(TargetFile, OutputCli);

                var savedPanel = new Panel($"[bold green]YAML file saved in:[/] {OutputCli}");

                savedPanel.Border = BoxBorder.Rounded;
                savedPanel.BorderStyle = new Style(Color.Green);

                AnsiConsole.Write(savedPanel);
            }
            else
            {
                AnsiConsole.MarkupLine("[bold red]The output must be a .json/.yaml file[/]");
            }
        }
        else
        {
            AnsiConsole.MarkupLine("[bold green]Output will not be saved in a file[/]");
        }
    }
}