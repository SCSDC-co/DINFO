using dinfo.core.Helpers.DirTools;
using dinfo.core.Utils.Globals;
using dinfo.core.Utils.Help;
using Spectre.Console;

namespace dinfo.tui;

public static class Program
{
    public static void PrintDirectoryInfo(string targetDirectory)
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
            $"[bold green]Total size:[/] {DirectoryHelper.SizeToReturn()} {GlobalsUtils.sizeExtension}");

        infoPanel.Border = BoxBorder.Rounded;
        infoPanel.BorderStyle = new Style(Color.Green);
        infoPanel.Header = new PanelHeader("[bold green] INFO [/]");

        /*
         *  PERIMSSIONS
         */

        var perms = DirectoryHelper.GetDirectoryPermissions(targetDirectory);
        var permissionPanel = new Panel($"{perms}");

        permissionPanel.Border = BoxBorder.Rounded;
        permissionPanel.BorderStyle = new Style(Color.Green);
        permissionPanel.Header = new PanelHeader("[bold green] PERMISSIONS [/]");

        /*
         *  FILES EXTENSIONS
         */

        var extensionsPanel = new Panel(
            $"[bold green]File extensions:[/] {string.Join(", ", GlobalsUtils.FilesTypes)}");

        extensionsPanel.Border = BoxBorder.Rounded;
        extensionsPanel.BorderStyle = new Style(Color.Green);
        extensionsPanel.Header = new PanelHeader("[bold green] EXTENSIONS [/]");

        var infoColumns = new Grid();

        infoColumns.AddColumn();
        infoColumns.AddColumn();
        infoColumns.AddColumn();

        infoColumns.AddRow(infoPanel, permissionPanel, extensionsPanel);

        AnsiConsole.Write(infoColumns);

        if (GlobalsUtils.verbose)
        {
            var filesPanel =
                new Panel($"[bold green]{string.Join(", ", GlobalsUtils.Files)}[/] ");

            filesPanel.Border = BoxBorder.Rounded;
            filesPanel.BorderStyle = new Style(Color.Green);
            filesPanel.Header = new PanelHeader("[bold green] FILES [/]");

            AnsiConsole.Write(filesPanel);
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
                        PrintDirectoryInfo(arg);
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
            PrintDirectoryInfo(currentDirectory);
        }
    }
}
