using dinfo.core.Utils.Globals;
using Microsoft.Extensions.Logging;
using System.Text;
using static dinfo.core.Utils.RegularExpressions.RegexHelpers;

namespace dinfo.core.Helpers.FilesTools;

public class FilesHelper(ILogger<FilesHelper> logger)
{
    public async Task<int> CountLinesAsync(string fileName, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Counting the lines of code for file {fileName}", fileName);
        IEnumerable<string> lines = await File.ReadAllLinesAsync(fileName, cancellationToken);

        var totalLines = lines.Count();
        logger.LogDebug("Found {totalLines} lines", totalLines);

        return totalLines;
    }

    public async Task<int> GetCommentsLinesAsync(string fileName, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Counting the lines of comments for file {fileName}", fileName);
        IEnumerable<string> lines = await File.ReadAllLinesAsync(fileName, cancellationToken).ConfigureAwait(false);

        bool inMultiLineComment = false;
        int commentLines = 0;

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();

            if (inMultiLineComment)
            {
                commentLines++;
                if (
                    MultiLineCommentEnd().IsMatch(trimmedLine)
                    || MultiLineMarkupEnd().IsMatch(trimmedLine)
                )
                {
                    inMultiLineComment = false;
                }
            }
            else if (
                MultiLineCommentStart().IsMatch(trimmedLine)
                || MultiLineMarkupStart().IsMatch(trimmedLine)
            )
            {
                commentLines++;
                inMultiLineComment = true;
            }
            else if (
                SlashComment().IsMatch(trimmedLine)
                || HashComment().IsMatch(trimmedLine)
                || DashComment().IsMatch(trimmedLine)
                || SemicolonComment().IsMatch(trimmedLine)
            )
            {
                commentLines++;
            }
        }

        GlobalsUtils.TotalLinesComments += commentLines;
        return commentLines;
    }

    public async Task<int> GetBlankLinesAsync(string fileName, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Counting the blank lines for file {fileName}", fileName);
        IEnumerable<string> lines = await File.ReadAllLinesAsync(fileName, cancellationToken).ConfigureAwait(false);

        int blankLines = lines.Count(line => string.IsNullOrWhiteSpace(line));
        GlobalsUtils.TotalBlankLines += blankLines;

        return blankLines;
    }

    public void GetFileType(string fileName)
    {
        logger.LogDebug("Getting file type for file {fileName}", fileName);
        string? name = Path.GetFileName(fileName);

        if (name?.StartsWith('.') ?? false)
        {
            return;
        }

        string? extension = Path.GetExtension(name);
        if (!string.IsNullOrWhiteSpace(extension))
        {
            GlobalsUtils.FileTypes.Add(extension);
        }
    }

    public string GetFileTypeSingleFile(string fileName)
    {
        logger.LogDebug("Getting file type single file");
        string? name = Path.GetFileName(fileName);

        if (name?.StartsWith('.') ?? false)
        {
            return "N/A";
        }

        string? extension = Path.GetExtension(name);
        if (!string.IsNullOrWhiteSpace(extension))
        {
            return extension;
        }

        return "N/A";
    }

    public async Task<Encoding> GetEncodingAsync(string fileName, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Getting the encoding for file {fileName}", fileName);
        var bom = new byte[4];

        await using var file = new FileStream(fileName, FileMode.Open, FileAccess.Read);

        await file.ReadExactlyAsync(bom, 0, Math.Min(4, (int)file.Length), cancellationToken).ConfigureAwait(false);

        if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf)
            return Encoding.UTF8;

        if (bom[0] == 0xff && bom[1] == 0xfe && bom[2] == 0 && bom[3] == 0)
            return Encoding.UTF32;

        if (bom[0] == 0xff && bom[1] == 0xfe)
            return Encoding.Unicode;

        if (bom[0] == 0xfe && bom[1] == 0xff)
            return Encoding.BigEndianUnicode;

        if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff)
            return new UTF32Encoding(true, true);

        var allBytes = await File.ReadAllBytesAsync(fileName, cancellationToken).ConfigureAwait(false);
        return IsUtf8(allBytes) ? Encoding.UTF8 : Encoding.ASCII;
    }

    private bool IsUtf8(byte[] bytes)
    {
        try
        {
            Encoding.UTF8.GetString(bytes);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Not an utf-8 string");
            return false;
        }
    }

    public void GetLastModifiedFile(string targetDirectory)
    {
        logger.LogDebug("Getting last modified file from directory {targetDirectory}", targetDirectory);
        var files = new DirectoryInfo(targetDirectory).EnumerateFiles().ToArray();

        if (files.Length == 0)
        {
            GlobalsUtils.LastModifiedFile = "N/A";
            return;
        }

        GlobalsUtils.LastModifiedFile = files.OrderBy(fi => fi.LastWriteTime).Last().FullName;
    }

    public async Task ProcessFileAsync(string fileName, CancellationToken cancellationToken = default)
    {
        try
        {
            GlobalsUtils.TotalFiles++;
            GlobalsUtils.TotalLines += await CountLinesAsync(fileName, cancellationToken).ConfigureAwait(false);
            await GetCommentsLinesAsync(fileName, cancellationToken).ConfigureAwait(false);
            await GetBlankLinesAsync(fileName, cancellationToken).ConfigureAwait(false);
            GlobalsUtils.Files.Add(fileName);

            var encoding = await GetEncodingAsync(fileName, cancellationToken).ConfigureAwait(false);
            GlobalsUtils.Encodings.Add(encoding.WebName);
        }
        catch (IOException ex)
        {
            logger.LogError(ex, "[SKIPPED] {fileName} (file locked by system)", fileName);
            Console.WriteLine($"[SKIPPED] {fileName} (file locked by system)");
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogError(ex, "[SKIPPED] {fileName} (access denied)", fileName);
            Console.WriteLine($"[SKIPPED] {fileName} (access denied)");
        }
    }
}
