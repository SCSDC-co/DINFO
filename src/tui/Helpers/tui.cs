using dinfo.core.Helpers.DirTools;
using dinfo.core.Helpers.FilesTools;
using dinfo.core.Helpers.GitTools;
using dinfo.core.Utils.Globals;
using Spectre.Console;

namespace dinfo.tui.Helpers.Tui;

public class TuiHelper(DirectoryHelper directoryHelper, GitHelper gitHelper, FilesHelper filesHelper)
{
    public async Task<Panel> BuildGitPanelAsync(string targetDirectory, CancellationToken cancellationToken = default)
    {
        await gitHelper.GetGitInfoAsync(targetDirectory, cancellationToken).ConfigureAwait(false);

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

    public async Task PrintDirectoryInfoAsync(string targetDirectory, CancellationToken cancellationToken = default)
    {
        await directoryHelper.ProcessDirectoryAsync(targetDirectory, cancellationToken).ConfigureAwait(false);

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

        var perms = directoryHelper.GetDirectoryPermissions(targetDirectory);

        int linesOfCode = GlobalsUtils.GetLinesOfCode();

        var infoPanel = new Panel(
            $"[bold green]Number of files:[/] {GlobalsUtils.TotalFiles}\n"
                + $"[bold green]Number of lines:[/] {GlobalsUtils.TotalLines}\n"
                + $"[bold green]Commentes:[/] {GlobalsUtils.TotalLinesComments}\n"
                + $"[bold green]Blank lines:[/] {GlobalsUtils.TotalBlankLines}\n"
                + $"[bold green]Code:[/] {linesOfCode}\n"
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

        var mostUsedExtension = GlobalsUtils.GetMostUsedExtension();

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

        infoColumns.AddRow(infoPanel, extensionsPanel, await BuildGitPanelAsync(targetDirectory, cancellationToken).ConfigureAwait(false));

        AnsiConsole.Write(infoColumns);

        if (GlobalsUtils.Verbose)
        {
            var filesPanel = new Panel($"[bold green]{string.Join(", ", GlobalsUtils.Files)}[/]");

            filesPanel.Border = BoxBorder.Rounded;
            filesPanel.BorderStyle = new Style(Color.Green);
            filesPanel.Header = new PanelHeader("[bold green] FILES [/]");

            AnsiConsole.Write(filesPanel);
        }

        /*
         *  SKIPPED FILES
         */

        if (GlobalsUtils.SkippedFileLocked == null)
        {
            var skippedFileLocked = new Panel($"[bold green]{string.Join(", ", GlobalsUtils.SkippedFileLocked ?? [])}[/]");

            skippedFileLocked.Border = BoxBorder.Rounded;
            skippedFileLocked.BorderStyle = new Style(Color.Yellow);
            skippedFileLocked.Header = new PanelHeader("[bold yellow]FILES SKIPPED (locked by system)[/]");

            AnsiConsole.Write(skippedFileLocked);
        }

        if (GlobalsUtils.SkippedFileAccesDenied == null)
        {
            var skippedFileAccessDenied = new Panel($"[bold green]{string.Join(", ", GlobalsUtils.SkippedFileAccesDenied ?? [])}[/]");

            skippedFileAccessDenied.Border = BoxBorder.Rounded;
            skippedFileAccessDenied.BorderStyle = new Style(Color.Yellow);
            skippedFileAccessDenied.Header = new PanelHeader("[bold yellow]FILES SKIPPED (access denied)[/]");

            AnsiConsole.Write(skippedFileAccessDenied);
        }
    }

    public async Task PrintFileInfoAsync(string targetFile, CancellationToken cancellationToken = default)
    {
        await filesHelper.ProcessFileAsync(targetFile, cancellationToken).ConfigureAwait(false);
        filesHelper.GetFileType(targetFile);

        /*
         *  HEADER
         */

        var headerPanel = new Panel($"[bold green]FINFO: {targetFile}[/]");

        headerPanel.Border = BoxBorder.Rounded;
        headerPanel.Expand = true;
        headerPanel.BorderStyle = new Style(Color.Green);

        AnsiConsole.Write(headerPanel);

        /*
         *  INFO
         */

        var lines = await filesHelper.CountLinesAsync(targetFile, cancellationToken).ConfigureAwait(false);
        var comments = await filesHelper.GetCommentsLinesAsync(targetFile, cancellationToken).ConfigureAwait(false);
        var blanks = await filesHelper.GetBlankLinesAsync(targetFile, cancellationToken).ConfigureAwait(false);
        var code = lines - (comments + blanks);

        var infoPanel = new Panel(
            $"[bold green]Number of lines:[/] {lines.ToString()}\n"
                + $"[bold green]Commentes:[/] {comments}\n"
                + $"[bold green]Blank lines:[/] {blanks}\n"
                + $"[bold green]Code:[/] {code}\n"
                + $"[bold green]File encoding:[/] {string.Join(", ", GlobalsUtils.Encodings.Distinct())}\n"
                + $"[bold green]File type:[/] {filesHelper.GetFileTypeSingleFile(targetFile)}"
        );

        infoPanel.Border = BoxBorder.Rounded;
        infoPanel.BorderStyle = new Style(Color.Green);
        infoPanel.Header = new PanelHeader("[bold green] INFO [/]");

        AnsiConsole.Write(infoPanel);
    }
}
