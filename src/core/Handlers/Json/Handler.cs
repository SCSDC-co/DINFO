using System.Text.Json;
using dinfo.core.Helpers.DirTools;
using dinfo.core.Helpers.FilesTools;
using dinfo.core.Helpers.GitTools;
using dinfo.core.Utils.Globals;

namespace dinfo.core.Handlers.Json;

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

        await File.WriteAllTextAsync(pathJson, jsonString);
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

        await File.WriteAllTextAsync(pathJson, jsonString);
    }
}
