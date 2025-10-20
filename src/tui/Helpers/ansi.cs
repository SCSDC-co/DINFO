using Spectre.Console;

namespace dinfo.tui.Helpers;

public static class AnsiConsoleHelper
{
    public static void Print(string outputCli)
    {
        var extension = Path.GetExtension(outputCli).ToLowerInvariant();

        switch (extension)
        {
            case ".json":
                AnsiConsole.Write(new Panel($"[bold green]JSON file saved in:[/] {outputCli}")
                    .Border(BoxBorder.Rounded)
                    .BorderStyle(new Style(Color.Green)));

                break;

            case ".yaml":
                AnsiConsole.Write(new Panel($"[bold green]YAML file saved in:[/] {outputCli}")
                    .Border(BoxBorder.Rounded)
                    .BorderStyle(new Style(Color.Green)));

                break;

            case ".htm":
            case ".html":
                AnsiConsole.Write(new Panel($"[bold green]HTML file saved in:[/] {outputCli}")
                    .Border(BoxBorder.Rounded)
                    .BorderStyle(new Style(Color.Green)));

                break;

            default:
                AnsiConsole.MarkupLine("[bold red]The output must be a .json/.yaml/.html file[/]");
                break;
        }
    }
}
