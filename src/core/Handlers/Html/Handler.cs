using dinfo.core.Helpers.DirTools;
using dinfo.core.Helpers.FilesTools;
using dinfo.core.Helpers.GitTools;
using dinfo.core.Interfaces.Output;
using dinfo.core.Utils.Globals;
using HtmlAgilityPack;

namespace dinfo.core.Handlers.Html;

public class HtmlHandler : IOutputHandler
{
    private static void AddRow(HtmlNode table, string property, string value)
    {
        var tr = HtmlNode.CreateNode("<tr></tr>");

        tr.AppendChild(HtmlNode.CreateNode($"<td>{HtmlEntity.Entitize(property)}</td>"));
        tr.AppendChild(HtmlNode.CreateNode($"<td>{HtmlEntity.Entitize(value)}</td>"));

        table.AppendChild(tr);
    }

    public async Task DirectorySaveAsync(string targetDirectory, string filePath, CancellationToken cancellationToken = default)
    {
        if (GlobalsUtils.NoTui)
        {
            await DirectoryHelper.ProcessDirectoryAsync(targetDirectory, cancellationToken).ConfigureAwait(false);
            await GitHelper.GetGitInfoAsync(targetDirectory, cancellationToken).ConfigureAwait(false);
        }

        string directorySize = $"{DirectoryHelper.SizeToReturn()} {GlobalsUtils.SizeExtension}";

        var mostUsedExtension = GlobalsUtils.GetMostUsedExtension();

        var perms = DirectoryHelper.GetDirectoryPermissions(targetDirectory);

        var doc = new HtmlDocument();

        doc.LoadHtml(
            $"<html><head><title>DINFO: {targetDirectory}</title></head><body></body></html>"
        );

        var body = doc.DocumentNode.SelectSingleNode("//body");

        /*
         *  TABLE 1 GENERAL DATA
         */

        var table = HtmlNode.CreateNode("<table></table>");
        body.AppendChild(table);

        var trHead = HtmlNode.CreateNode("<tr><th>Property</th><th>Value</th></tr>");
        table.AppendChild(trHead);

        AddRow(table, "Path", targetDirectory);
        AddRow(table, "BiggestFile", GlobalsUtils.BiggestFile);
        AddRow(table, "LatestModifiedFile", GlobalsUtils.LastModifiedFile);
        AddRow(table, "FileTypes", string.Join(", ", GlobalsUtils.FileTypes.Distinct()));
        AddRow(table, "MostUsedExtension", mostUsedExtension.TrimStart('.'));
        AddRow(table, "Encoding", string.Join(", ", GlobalsUtils.Encodings));
        AddRow(table, "Files", GlobalsUtils.TotalFiles.ToString());
        AddRow(table, "Size", directorySize);
        AddRow(table, "Permissions", perms);
        AddRow(table, "Lines", GlobalsUtils.TotalLines.ToString());
        AddRow(table, "Code", GlobalsUtils.TotalLinesCode.ToString());
        AddRow(table, "Comments", GlobalsUtils.TotalLinesComments.ToString());
        AddRow(table, "Blank", GlobalsUtils.TotalBlankLines.ToString());
        AddRow(table, "Directories", GlobalsUtils.TotalDirs.ToString());

        /*
         *  TABLE 2 GIT DATA
         */

        var tableGit = HtmlNode.CreateNode("<table></table>");
        body.AppendChild(tableGit);

        var trHeadGit = HtmlNode.CreateNode("<tr><th>Property</th><th>Value</th></tr>");
        tableGit.AppendChild(trHeadGit);

        AddRowGit("GitBranchName", GlobalsUtils.GitBranchName);
        AddRowGit("GitHash", GlobalsUtils.GitHash);
        AddRowGit("GitAuthor", GlobalsUtils.GitAuthor);
        AddRowGit("GitCommit", GlobalsUtils.GitCommitter);
        AddRowGit("GitSubject", GlobalsUtils.GitSubject);

        doc.Save(filePath);

        void AddRowGit(string property, string value)
        {
            var tr = HtmlNode.CreateNode("<tr></tr>");
            tr.AppendChild(HtmlNode.CreateNode($"<td>{HtmlEntity.Entitize(property)}</td>"));
            tr.AppendChild(HtmlNode.CreateNode($"<td>{HtmlEntity.Entitize(value)}</td>"));
            tableGit.AppendChild(tr);
        }
    }

    public async Task FileSaveAsync(string targetFile, string filePath, CancellationToken cancellationToken = default)
    {
        if (GlobalsUtils.NoTui)
        {
            await FilesHelper.ProcessFileAsync(targetFile, cancellationToken).ConfigureAwait(false);
        }

        var lines = await FilesHelper.CountLinesAsync(targetFile, cancellationToken).ConfigureAwait(false);
        var comments = await FilesHelper.GetCommentsLinesAsync(targetFile, cancellationToken).ConfigureAwait(false);
        var blank = await FilesHelper.GetBlankLinesAsync(targetFile, cancellationToken).ConfigureAwait(false);
        var code = lines - (comments + blank);

        var doc = new HtmlDocument();

        doc.LoadHtml(
            $"<html><head><title>FINFO: {targetFile}</title></head><body></body></html>"
        );

        var body = doc.DocumentNode.SelectSingleNode("//body");

        var table = HtmlNode.CreateNode("<table></table>");
        body.AppendChild(table);

        var trHead = HtmlNode.CreateNode("<tr><th>Property</th><th>Value</th></tr>");
        table.AppendChild(trHead);

        AddRow(table, "FileName", targetFile);
        AddRow(table, "Lines", lines.ToString());
        AddRow(table, "Comments", comments.ToString());
        AddRow(table, "Code", code.ToString());
        AddRow(table, "Blanks", blank.ToString());
        AddRow(table, "Encoding", GlobalsUtils.Encodings.Distinct().ToString() ?? string.Empty);
        AddRow(table, "FileTypes", FilesHelper.GetFileTypeSingleFile(targetFile));

        doc.Save(filePath);
    }
}
