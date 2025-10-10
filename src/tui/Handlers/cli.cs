using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using dinfo.core.Handlers.Json;
using dinfo.core.Handlers.Yaml;
using dinfo.core.Handlers.Html;
using dinfo.core.Utils.Globals;
using dinfo.tui.Helpers.Tui;
using Spectre.Console;

[Command(Description = "Display information about the specified directory and its contents.")]
public class DinfoCommand : ICommand
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
            switch (Path.GetExtension(OutputCli).ToLowerInvariant())
            {
                case ".json":
                    await JsonHandler.DirectorySaveJsonAsync(dir, OutputCli);

                    AnsiConsole.Write(new Panel($"[bold green]JSON file saved in:[/] {OutputCli}")
                        .Border(BoxBorder.Rounded)
                        .BorderStyle(new Style(Color.Green)));

                    break;

                case ".yaml":
                    await YamlHandler.DirectorySaveYamlAsync(dir, OutputCli);

                    AnsiConsole.Write(new Panel($"[bold green]YAML file saved in:[/] {OutputCli}")
                        .Border(BoxBorder.Rounded)
                        .BorderStyle(new Style(Color.Green)));

                    break;

                case ".html":
                    await HtmlHandler.DirectorySaveHtml(dir, OutputCli);

                    AnsiConsole.Write(new Panel($"[bold green]HTML file saved in:[/] {OutputCli}")
                        .Border(BoxBorder.Rounded)
                        .BorderStyle(new Style(Color.Green)));

                    break;

                default:
                    AnsiConsole.MarkupLine("[bold red]The output must be a .json/.yaml/.html file[/]");
                    break;
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
        Description = "Specify the output file (formats: json, yaml, html)."
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
            switch (Path.GetExtension(OutputCli).ToLowerInvariant())
            {
                case ".json":
                    await JsonHandler.FileSaveJsonAsync(TargetFile, OutputCli);

                    AnsiConsole.Write(new Panel($"[bold green]JSON file saved in:[/] {OutputCli}")
                        .Border(BoxBorder.Rounded)
                        .BorderStyle(new Style(Color.Green)));

                    break;

                case ".yaml":
                    await YamlHandler.FileSaveYamlAsync(TargetFile, OutputCli);

                    AnsiConsole.Write(new Panel($"[bold green]YAML file saved in:[/] {OutputCli}")
                        .Border(BoxBorder.Rounded)
                        .BorderStyle(new Style(Color.Green)));

                    break;

                case ".html":
                    await HtmlHandler.FileSaveHtmlAsync(TargetFile, OutputCli);

                    AnsiConsole.Write(new Panel($"[bold green]HTML file saved in:[/] {OutputCli}")
                        .Border(BoxBorder.Rounded)
                        .BorderStyle(new Style(Color.Green)));

                    break;

                default:
                    AnsiConsole.MarkupLine("[bold red]The output must be a .json/.yaml/.html file[/]");
                    break;
            }
        }
        else
        {
            AnsiConsole.MarkupLine("[bold green]Output will not be saved in a file[/]");
        }
    }
}
