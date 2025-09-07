using dinfo.core.Helpers.DirTools;
using dinfo.core.Utils.Globals;
using Spectre.Console;

namespace dinfo.tui.Helpers.tui;

public static class TuiHelper
{
    public static async Task<Panel> BuildGitPanel(string targetDirectory)
    {
        await dinfo.core.Helpers.GitTools.GitHelper.GetGitInfo(targetDirectory);

        if (GlobalsUtils.IsRepo)
        {
            var gitPanel = new Panel(
                $"[bold green]Git Branch Name:[/] {GlobalsUtils.GitBranchName}\n" +
                $"[bold green]Git Hash:[/] {GlobalsUtils.GitHash}\n" +
                $"[bold green]Git Author:[/] {GlobalsUtils.GitAuthor}\n" +
                $"[bold green]Git Committer:[/] {GlobalsUtils.GitCommitter}\n" +
                $"[bold green]Git Subject:[/] {GlobalsUtils.GitSubject}"
            );

            gitPanel.Border = BoxBorder.Rounded;
            gitPanel.BorderStyle = new Style(Color.Green);
            gitPanel.Header = new PanelHeader("[bold green] GIT [/]");

            return gitPanel;
        }
        else
        {
            var gitPanel = new Panel("[bold red]Not a git repository[/]");

            gitPanel.Border = BoxBorder.Rounded;
            gitPanel.BorderStyle = new Style(Color.Red);
            gitPanel.Header = new PanelHeader("[bold red] GIT [/]");

            return gitPanel;
        }
    }

    public static async Task PrintDirectoryInfo(string targetDirectory)
    {
        await DirectoryHelper.ProcessDirectory(targetDirectory);

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

        var perms = DirectoryHelper.GetDirectoryPermissions(targetDirectory);

        var infoPanel = new Panel(
            $"[bold green]Number of files:[/] {GlobalsUtils.TotalFiles}\n" +
            $"[bold green]Number of lines:[/] {GlobalsUtils.TotalLines}\n" +
            $"[bold green]Number of directories:[/] {GlobalsUtils.TotalDirs}\n" +
            $"[bold green]Permissions:[/] {perms}\n" +
            $"[bold green]Total size:[/] {DirectoryHelper.SizeToReturn()} {GlobalsUtils.SizeExtension}"
        );

        infoPanel.Border = BoxBorder.Rounded;
        infoPanel.BorderStyle = new Style(Color.Green);
        infoPanel.Header = new PanelHeader("[bold green] INFO [/]");

        /*
		 *  FILES
		 */

        var fileTypesNoDupes = GlobalsUtils.FileTypes.Distinct().ToList();

        var mostUsedExtension = GlobalsUtils.FileTypes
            .GroupBy(x => x)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefault() ?? "N/A";

        var extensionsPanel = new Panel(
            $"[bold green]File extensions:[/] {string.Join(", ", fileTypesNoDupes)}\n" +
            $"[bold green]Most used extension:[/] {mostUsedExtension.TrimStart('.')}\n" +
            $"[bold green]Biggest file:[/] {GlobalsUtils.BiggestFile} ({GlobalsUtils.BiggestFileSize} B)\n" +
            $"[bold green]Last modified file:[/] {GlobalsUtils.LastModifiedFile}\n" +
            $"[bold green]File encodings:[/] {string.Join(", ", GlobalsUtils.Encodings.Distinct())}"
        );

        extensionsPanel.Border = BoxBorder.Rounded;
        extensionsPanel.BorderStyle = new Style(Color.Green);
        extensionsPanel.Header = new PanelHeader("[bold green] FILES [/]");

        /*
		 *  GRID
		 */

        var infoColumns = new Grid();

        infoColumns.AddColumn();
        infoColumns.AddColumn();
        infoColumns.AddColumn();

        infoColumns.AddRow(infoPanel, extensionsPanel, await BuildGitPanel(targetDirectory));

        AnsiConsole.Write(infoColumns);

        if (GlobalsUtils.Verbose)
        {
            var filesPanel =
                new Panel($"[bold green]{string.Join(", ", GlobalsUtils.Files)}[/] ");

            filesPanel.Border = BoxBorder.Rounded;
            filesPanel.BorderStyle = new Style(Color.Green);
            filesPanel.Header = new PanelHeader("[bold green] FILES [/]");

            AnsiConsole.Write(filesPanel);
        }
    }

    public static void PrintHelp()
    {
        var optionsTableHelp = new Table();

        optionsTableHelp.AddColumn("[bold green]OPTION[/]");
        optionsTableHelp.AddColumn("[bold green]DESCRIPTION[/]");

        optionsTableHelp.AddRow("[bold green]-r   --Recursive[/]  ", "Recursively list all files and directories.");
        optionsTableHelp.AddRow("[bold green]-v   --Verbose[/]  ", "Enable verbose output.");
        optionsTableHelp.AddRow("[bold green]-ig  --Ignore-Gitignore[/]  ", "Ignore .gitignore files.");
        optionsTableHelp.AddRow("[bold green]-h   --help[/]  ", "Show help information.");

        optionsTableHelp.Border = TableBorder.None;

        var headerPanelHelp = new Panel(new Rows(
            new Markup("[bold green]USAGE:[/] dinfo [[options]] [[directory]]\n"),
            new Markup("[bold green]DESCRIPTION:[/]\n     Display information about the specified directory and its contents.\n"),
            optionsTableHelp
        ));


        headerPanelHelp.Border = BoxBorder.Rounded;
        headerPanelHelp.BorderStyle = new Style(Color.Green);
        headerPanelHelp.Header = new PanelHeader("[bold green] HELP [/]");

        AnsiConsole.Write(headerPanelHelp);
    }
}