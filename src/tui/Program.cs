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
            $"[bold green]Number of files:[/] {GlobalsUtils.totalFiles}\n[bold green]Number of lines:[/] {GlobalsUtils.totalLines}\n[bold green]Number of directories:[/] {GlobalsUtils.totalDirs}"
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

        if (args.Length == 0)
        {
            DirectoryInfo(currentDirectory);
        }

        if (args.Length == 1 && (args[0] == "-h" || args[0] == "--help"))
        {
            HelpUtils.PrintHelp();
            return;
        }

        foreach (string arg in args)
        {
            if (arg == "-r" || arg == "--recursive")
            {
                GlobalsUtils.recursive = true;
                DirectoryInfo(currentDirectory);
                continue;
            }

            if (arg == "-v" || arg == "--verbose")
            {
                GlobalsUtils.verbose = true;
                DirectoryInfo(currentDirectory);
                continue;
            }

            if (!Directory.Exists(arg))
            {
                Console.WriteLine($"Invalid input, skipping {arg}");
                continue;
            }

            var attributes = File.GetAttributes(arg);

            if (attributes.HasFlag(FileAttributes.Directory))
            {
                DirectoryInfo(arg);
            }
            else
            {
                Console.WriteLine($"Invalid input, skipping {arg}");
            }
        }
    }
}
