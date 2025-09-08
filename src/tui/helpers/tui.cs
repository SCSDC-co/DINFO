using dinfo.core.Helpers.DirTools;
using dinfo.core.Utils.Globals;
using Spectre.Console;

namespace dinfo.tui.Helpers.Tui;

public static class TuiHelper
{
    public static async Task<Panel> BuildGitPanel(string targetDirectory)
    {
        await dinfo.core.Helpers.GitTools.GitHelper.GetGitInfo(targetDirectory);

        if (GlobalsUtils.IsRepo)
        {
            var gitPanel = new Panel(
                $"[bold green]Git Branch Name:[/] {GlobalsUtils.GitBranchName}\n"
                    + $"[bold green]Git Hash:[/] {GlobalsUtils.GitHash}\n"
                    + $"[bold green]Git Author:[/] {GlobalsUtils.GitAuthor}\n"
                    + $"[bold green]Git Committer:[/] {GlobalsUtils.GitCommitter}\n"
                    + $"[bold green]Git Subject:[/] {GlobalsUtils.GitSubject}"
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
            $"[bold green]Number of files:[/] {GlobalsUtils.TotalFiles}\n"
                + $"[bold green]Number of lines:[/] {GlobalsUtils.TotalLines}\n"
                + $"[bold green]Number of directories:[/] {GlobalsUtils.TotalDirs}\n"
                + $"[bold green]Permissions:[/] {perms}\n"
                + $"[bold green]Total size:[/] {DirectoryHelper.SizeToReturn()} {GlobalsUtils.SizeExtension}"
        );

        infoPanel.Border = BoxBorder.Rounded;
        infoPanel.BorderStyle = new Style(Color.Green);
        infoPanel.Header = new PanelHeader("[bold green] INFO [/]");

        /*
         *  FILES
         */

        var fileTypesNoDupes = GlobalsUtils.FileTypes.Distinct().ToList();

        var mostUsedExtension =
            GlobalsUtils
                .FileTypes.GroupBy(x => x)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault()
            ?? "N/A";

        var extensionsPanel = new Panel(
            $"[bold green]File extensions:[/] {string.Join(", ", fileTypesNoDupes)}\n"
                + $"[bold green]Most used extension:[/] {mostUsedExtension.TrimStart('.')}\n"
                + $"[bold green]Biggest file:[/] {GlobalsUtils.BiggestFile} ({GlobalsUtils.BiggestFileSize} B)\n"
                + $"[bold green]Last modified file:[/] {GlobalsUtils.LastModifiedFile}\n"
                + $"[bold green]File encodings:[/] {string.Join(", ", GlobalsUtils.Encodings.Distinct())}"
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
            var filesPanel = new Panel($"[bold green]{string.Join(", ", GlobalsUtils.Files)}[/] ");

            filesPanel.Border = BoxBorder.Rounded;
            filesPanel.BorderStyle = new Style(Color.Green);
            filesPanel.Header = new PanelHeader("[bold green] FILES [/]");

            AnsiConsole.Write(filesPanel);
        }
    }
}
