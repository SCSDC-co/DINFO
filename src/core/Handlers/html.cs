using HtmlAgilityPack;
using dinfo.core.Helpers.DirTools;
using dinfo.core.Helpers.FilesTools;
using dinfo.core.Helpers.GitTools;
using dinfo.core.Utils.Globals;

namespace dinfo.core.Handlers.Html;

public static class HtmlHandler
{
    public static async Task DirectorySaveHtml(string targetDirectory, string pathHtml)
    {
        if (GlobalsUtils.NoTui)
        {
            await DirectoryHelper.ProcessDirectoryAsync(targetDirectory);
            await GitHelper.GetGitInfoAsync(targetDirectory);
        }

        string directorySize =
            (DirectoryHelper.SizeToReturn()).ToString() + " " + GlobalsUtils.SizeExtension;

        var mostUsedExtension =
            GlobalsUtils
                .FileTypes.GroupBy(x => x)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault()
            ?? "N/A";

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

        void AddRow(string property, string value)
        {
            var tr = HtmlNode.CreateNode("<tr></tr>");
            tr.AppendChild(HtmlNode.CreateNode($"<td>{HtmlEntity.Entitize(property)}</td>"));
            tr.AppendChild(HtmlNode.CreateNode($"<td>{HtmlEntity.Entitize(value)}</td>"));
            table.AppendChild(tr);
        }

        AddRow("Path", targetDirectory);
        AddRow("BiggestFile", GlobalsUtils.BiggestFile);
        AddRow("LatestModifiedFile", GlobalsUtils.LastModifiedFile);
        AddRow("FileTypes", string.Join(", ", GlobalsUtils.FileTypes.Distinct()));
        AddRow("MostUsedExtension", mostUsedExtension.TrimStart('.'));
        AddRow("Encoding", string.Join(", ", GlobalsUtils.Encodings));
        AddRow("Files", GlobalsUtils.TotalFiles.ToString());
        AddRow("Size", directorySize);
        AddRow("Permissions", perms);
        AddRow("Lines", GlobalsUtils.TotalLines.ToString());
        AddRow("Code", GlobalsUtils.TotalLinesCode.ToString());
        AddRow("Comments", GlobalsUtils.TotalLinesComments.ToString());
        AddRow("Blank", GlobalsUtils.TotalBlankLines.ToString());
        AddRow("Directories", GlobalsUtils.TotalDirs.ToString());

        /*
         *  TABLE 2 GIT DATA
         */

        var tableGit = HtmlNode.CreateNode("<table></table>");
        body.AppendChild(tableGit);

        var trHeadGit = HtmlNode.CreateNode("<tr><th>Property</th><th>Value</th></tr>");
        tableGit.AppendChild(trHeadGit);

        void AddRowGit(string property, string value)
        {
            var tr = HtmlNode.CreateNode("<tr></tr>");
            tr.AppendChild(HtmlNode.CreateNode($"<td>{HtmlEntity.Entitize(property)}</td>"));
            tr.AppendChild(HtmlNode.CreateNode($"<td>{HtmlEntity.Entitize(value)}</td>"));
            tableGit.AppendChild(tr);
        }

        AddRowGit("GitBranchName", GlobalsUtils.GitBranchName);
        AddRowGit("GitHash", GlobalsUtils.GitHash);
        AddRowGit("GitAuthor", GlobalsUtils.GitAuthor);
        AddRowGit("GitCommit", GlobalsUtils.GitCommitter);
        AddRowGit("GitSubject", GlobalsUtils.GitSubject);

        doc.Save(pathHtml);
    }

    public static async Task FileSaveHtmlAsync(string targetFile, string pathHtml)
    {
        if (GlobalsUtils.NoTui)
        {
            await FilesHelper.ProcessFileAsync(targetFile);
        }

        var lines = await FilesHelper.CountLinesAsync(targetFile);
        var comments = await FilesHelper.GetCommentsLinesAsync(targetFile);
        var blank = await FilesHelper.GetBlankLinesAsync(targetFile);
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

        void AddRow(string property, string value)
        {
            var tr = HtmlNode.CreateNode("<tr></tr>");
            tr.AppendChild(HtmlNode.CreateNode($"<td>{HtmlEntity.Entitize(property)}</td>"));
            tr.AppendChild(HtmlNode.CreateNode($"<td>{HtmlEntity.Entitize(value)}</td>"));
            table.AppendChild(tr);
        }

        AddRow("FileName", targetFile);
        AddRow("Lines", lines.ToString());
        AddRow("Comments", comments.ToString());
        AddRow("Code", code.ToString());
        AddRow("Blanks", blank.ToString());
        AddRow("Encoding", GlobalsUtils.Encodings.Distinct().ToString());
        AddRow("FileTypes", FilesHelper.GetFileTypeSingleFile(targetFile));

        doc.Save(pathHtml);
    }
}
