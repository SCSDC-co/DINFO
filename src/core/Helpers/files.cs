using System.Text.RegularExpressions;
using System.Text;

using dinfo.core.Utils.Globals;

namespace dinfo.core.Helpers.FilesTools;

public static class FilesHelper
{
    public static async Task<int> CountLines(string fileName)
    {
        IEnumerable<string> lines = await File.ReadAllLinesAsync(fileName);

        return lines.Count();
    }

    public static async Task<int> GetCommentsLines(string fileName)
    {
        var slashComment = new Regex(@"^\s*//");
        var hashComment = new Regex(@"^\s*#");
        var multiLineCommentStart = new Regex(@"^\s*/\*");
        var multiLineCommentEnd = new Regex(@"^\s*\*/");
        var multiLineMarkupStart = new Regex(@"^\s*<!--");
        var multiLineMarkupEnd = new Regex(@"^\s*-->$");
        var dashComment = new Regex(@"^\s*--");
        var semicolonComment = new Regex(@"^\s*;");

        IEnumerable<string> lines = await File.ReadAllLinesAsync(fileName);

        bool inMultiLineComment = false;
        int commentLines = 0;

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();

            if (inMultiLineComment)
            {
                commentLines++;
                if (multiLineCommentEnd.IsMatch(trimmedLine) || multiLineMarkupEnd.IsMatch(trimmedLine))
                {
                    inMultiLineComment = false;
                }
            }
            else if (multiLineCommentStart.IsMatch(trimmedLine) || multiLineMarkupStart.IsMatch(trimmedLine))
            {
                commentLines++;
                inMultiLineComment = true;
            }
            else if (slashComment.IsMatch(trimmedLine) || hashComment.IsMatch(trimmedLine) || dashComment.IsMatch(trimmedLine) || semicolonComment.IsMatch(trimmedLine))
            {
                commentLines++;
            }
        }

        GlobalsUtils.TotalLinesComments += commentLines;

        return commentLines;
    }

    public static void GetFileType(string fileName)
    {
        string name = Path.GetFileName(fileName);

        if (name.StartsWith("."))
        {
            return;
        }

        string ext = Path.GetExtension(name);

        if (!string.IsNullOrEmpty(ext))
        {
            GlobalsUtils.FileTypes.Add(ext);
        }
    }

    public static string GetFileTypeSingleFile(string fileName)
    {
        string name = Path.GetFileName(fileName);

        if (name.StartsWith("."))
        {
            return "N/A";
        }

        string ext = Path.GetExtension(name);

        if (!string.IsNullOrEmpty(ext))
        {
            return ext;
        }

        return "N/A";
    }

    public static Encoding GetEncoding(string fileName)
    {
        var bom = new byte[4];
        using (var file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        {
            file.ReadExactlyAsync(bom, 0, Math.Min(4, (int)file.Length));
        }

        if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
        if (bom[0] == 0xff && bom[1] == 0xfe && bom[2] == 0 && bom[3] == 0) return Encoding.UTF32;
        if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode;
        if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode;
        if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return new UTF32Encoding(true, true);

        var allBytes = File.ReadAllBytes(fileName);
        if (IsUtf8(allBytes)) return Encoding.UTF8;

        return Encoding.ASCII;
    }

    private static bool IsUtf8(byte[] bytes)
    {
        try
        {
            Encoding.UTF8.GetString(bytes);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static void GetLastModifiedFile(string targetDirectory)
    {
        var files = new DirectoryInfo(targetDirectory).EnumerateFiles().ToArray();

        if (files.Length == 0)
        {
            GlobalsUtils.LastModifiedFile = "N/A";
            return;
        }

        GlobalsUtils.LastModifiedFile = files
            .OrderBy(fi => fi.LastWriteTime)
            .Last()
            .FullName;
    }

    public static async Task ProcessFile(string fileName)
    {

        try
        {
            GlobalsUtils.TotalFiles++;
            GlobalsUtils.TotalLines += await CountLines(fileName);
            GlobalsUtils.Files.Add(fileName);
            GlobalsUtils.Encodings.Add(GetEncoding(fileName).WebName);
        }
        catch (IOException)
        {
            Console.WriteLine($"[SKIPPED] {fileName} (file locked by system)");
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine($"[SKIPPED] {fileName} (access denied)");
        }
    }
}
