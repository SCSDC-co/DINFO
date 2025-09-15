using System.Text.Json;
using dinfo.core.Helpers.DirTools;
using dinfo.core.Helpers.FilesTools;
using dinfo.core.Helpers.GitTools;
using dinfo.core.Utils.Globals;

namespace dinfo.core.Handlers.Json;

public class DirectoryJson
{
    public string Path { get; set; } = "N/A";
    public string BiggestFile { get; set; } = "N/A";
    public string LatestModifiedFile { get; set; } = "N/A";
    public string Size { get; set; } = "N/A";
    public string Permissions { get; set; } = "N/A";
    public string MostUsedExtension { get; set; } = "N/A";
    public List<string> FileType { get; set; } = ["N/A"];
    public List<string> Encoding { get; set; } = ["N/A"];
    public int Files { get; set; }
    public int Directories { get; set; }
    public int Comments { get; set; }
    public int Blank { get; set; }
    public int Code { get; set; }
    public int Lines { get; set; }
    public required Dictionary<string, GitJson> Git { get; set; }
}

public class GitJson
{
    public string GitBranchName { get; set; } = "N/A";
    public string GitHash { get; set; } = "N/A";
    public string GitAuthor { get; set; } = "N/A";
    public string GitCommitter { get; set; } = "N/A";
    public string GitSubject { get; set; } = "N/A";
}

public class FileJson
{
    public string FileName { get; set; } = "N/A";
    public int Lines { get; set; } = 0;
    public int Comments { get; set; } = 0;
    public int Blanks { get; set; } = 0;
    public int Code { get; set; } = 0;
    public List<string> Encoding { get; set; } = ["N/A"];
    public string FileType { get; set; } = "N/A";
}

public static class JsonHandler
{
    public static async Task DirectorySaveJsonAsync(string targetDirectory, string pathJson)
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

        var json = new DirectoryJson
        {
            Path = targetDirectory,
            BiggestFile = GlobalsUtils.BiggestFile,
            LatestModifiedFile = GlobalsUtils.LastModifiedFile,
            FileType = GlobalsUtils.FileTypes.Distinct().ToList(),
            MostUsedExtension = mostUsedExtension.TrimStart('.'),
            Encoding = GlobalsUtils.Encodings,
            Files = GlobalsUtils.TotalFiles,
            Size = directorySize,
            Permissions = perms,
            Lines = GlobalsUtils.TotalLines,
            Code = GlobalsUtils.TotalLinesCode,
            Comments = GlobalsUtils.TotalLinesComments,
            Blank = GlobalsUtils.TotalBlankLines,
            Directories = GlobalsUtils.TotalDirs,
            Git = new Dictionary<string, GitJson>
            {
                ["Repo"] = new GitJson
                {
                    GitBranchName = GlobalsUtils.GitBranchName,
                    GitHash = GlobalsUtils.GitHash,
                    GitAuthor = GlobalsUtils.GitAuthor,
                    GitCommitter = GlobalsUtils.GitCommitter,
                    GitSubject = GlobalsUtils.GitSubject,
                },
            },
        };

        var options = new JsonSerializerOptions { WriteIndented = true };
        string jsonString = JsonSerializer.Serialize(json, options);

        File.WriteAllText(pathJson, jsonString);
    }

    public static async Task FileSaveJsonAsync(string targetFile, string pathJson)
    {
        if (GlobalsUtils.NoTui)
        {
            await FilesHelper.ProcessFileAsync(targetFile);
        }

        var lines = await FilesHelper.CountLinesAsync(targetFile);
        var comments = await FilesHelper.GetCommentsLinesAsync(targetFile);
        var blank = await FilesHelper.GetBlankLinesAsync(targetFile);
        var code = lines - (comments + blank);

        var json = new FileJson
        {
            FileName = targetFile,
            Lines = lines,
            Comments = comments,
            Code = code,
            Blanks = blank,
            Encoding = GlobalsUtils.Encodings.Distinct().ToList(),
            FileType = FilesHelper.GetFileTypeSingleFile(targetFile),
        };

        var options = new JsonSerializerOptions { WriteIndented = true };
        string jsonString = JsonSerializer.Serialize(json, options);

        File.WriteAllText(pathJson, jsonString);
    }
}
