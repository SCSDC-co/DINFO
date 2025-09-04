using dinfo.core.Helpers.DirTools;
using dinfo.core.Utils.Globals;
using dinfo.core.Utils.Help;
using Spectre.Console;

namespace dinfo.tui;

public static class Program
{
    public static void DirectoryInfo(string targetDirectory)
    {
        DirectoryHelper.ProcessDirectory(targetDirectory);

        /*
         *  HEADER
         */
        var headerPanel = new Panel($"[bold green]DINFO: {targetDirectory}[/]");

        headerPanel.Border = BoxBorder.Rounded;
        headerPanel.Expand = true;
        headerPanel.BorderStyle = new Style(Color.Green);

        AnsiConsole.Write(headerPanel);

        /*
         *  INFO
         */
        var infoPanel = new Panel(
            $"[bold green]Number of files:[/] {GlobalsUtils.totalFiles}\n" +
            $"[bold green]Number of lines:[/] {GlobalsUtils.totalLines}\n" +
            $"[bold green]Number of directories:[/] {GlobalsUtils.totalDirs}\n" +
            $"[bold green]Total size:[/] {DirectoryHelper.sizeToReturn()} {GlobalsUtils.sizeExtension}"
        );


        infoPanel.Border = BoxBorder.Rounded;
        infoPanel.BorderStyle = new Style(Color.Green);
        infoPanel.Header = new PanelHeader("[bold green] INFO [/]");

        if (!GlobalsUtils.verbose)
        {
            AnsiConsole.Write(infoPanel);
        }
        else
        {
            var filesPanel = new Panel($"[bold green]{string.Join(", ", GlobalsUtils.Files)}[/] ");

            filesPanel.Border = BoxBorder.Rounded;
            filesPanel.BorderStyle = new Style(Color.Green);
            filesPanel.Header = new PanelHeader("[bold green] FILES [/]");

            var infoColumns = new Grid();

            infoColumns.AddColumn();
            infoColumns.AddColumn();

            infoColumns.AddRow(infoPanel, filesPanel);

            AnsiConsole.Write(infoColumns);
        }
    }

    public static void Main(string[] args)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        bool hasDirectory = false;

        foreach (string arg in args)
        {
            switch (arg)
            {
                case "-r":
                case "--recursive":
                    GlobalsUtils.recursive = true;
                    break;

                case "-v":
                case "--verbose":
                    GlobalsUtils.verbose = true;
                    break;

                case "-h":
                case "--help":
                    HelpUtils.PrintHelp();
                    return;

                default:
                    if (Directory.Exists(arg))
                    {
                        DirectoryInfo(arg);
                        hasDirectory = true;
                    }
                    else
                    {
                        Console.WriteLine($"Invalid input, skipping {arg}");
                    }
                    break;
            }
        }

        if (!hasDirectory)
        {
            DirectoryInfo(currentDirectory);
        }
    }
}
