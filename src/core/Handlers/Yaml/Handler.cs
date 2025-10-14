using dinfo.core.Helpers.DirTools;
using dinfo.core.Helpers.FilesTools;
using dinfo.core.Helpers.GitTools;
using dinfo.core.Interfaces.Output;
using dinfo.core.Utils.Globals;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace dinfo.core.Handlers.Yaml;

public class YamlHandler : IOutputHandler
{
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

        await File.WriteAllTextAsync(filePath, yamlString, cancellationToken).ConfigureAwait(false);
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

        await File.WriteAllTextAsync(filePath, yamlString, cancellationToken).ConfigureAwait(false);
    }
}
