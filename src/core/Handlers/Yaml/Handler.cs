using dinfo.core.Helpers.DirTools;
using dinfo.core.Helpers.FilesTools;
using dinfo.core.Helpers.GitTools;
using dinfo.core.Utils.Globals;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using dinfo.core.Interfaces.Output;

namespace dinfo.core.Handlers.Yaml;

public class YamlHandler : IOutputHandler
{
    public async Task DirectorySaveAsync(string targetDirectory, string filePath, CancellationToken cancellationToken = default)
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

        var yaml = new DirectoryYaml
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
            Git = new Dictionary<string, GitYaml>
            {
                ["Repo"] = new GitYaml
                {
                    GitBranchName = GlobalsUtils.GitBranchName,
                    GitHash = GlobalsUtils.GitHash,
                    GitAuthor = GlobalsUtils.GitAuthor,
                    GitCommitter = GlobalsUtils.GitCommitter,
                    GitSubject = GlobalsUtils.GitSubject,
                },
            },
        };

        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        var yamlString = serializer.Serialize(yaml);

        await File.WriteAllTextAsync(filePath, yamlString);
    }

    public async Task FileSaveAsync(string targetFile, string filePath, CancellationToken cancellationToken = default)
    {
        if (GlobalsUtils.NoTui)
        {
            await FilesHelper.ProcessFileAsync(targetFile);
        }

        var lines = await FilesHelper.CountLinesAsync(targetFile);
        var comments = await FilesHelper.GetCommentsLinesAsync(targetFile);
        var blank = await FilesHelper.GetBlankLinesAsync(targetFile);
        var code = lines - (comments + blank);

        var yaml = new FileYaml
        {
            FileName = targetFile,
            Lines = lines,
            Comments = comments,
            Code = code,
            Blanks = blank,
            Encoding = GlobalsUtils.Encodings.Distinct().ToList(),
            FileType = FilesHelper.GetFileTypeSingleFile(targetFile),
        };

        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        var yamlString = serializer.Serialize(yaml);

        await File.WriteAllTextAsync(filePath, yamlString);
    }
}
