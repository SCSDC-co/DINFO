using dinfo.core.Utils.Globals;
using GitReader;
using GitReader.Structures;
using MAB.DotIgnore;
using Microsoft.Extensions.Logging;

namespace dinfo.core.Helpers.GitTools;

public class GitHelper(ILogger<GitHelper> logger)
{
    public bool IsFileIgnore(string path, FileInfo fileName)
    {
        logger.LogDebug("checking if file {path} is ignored", path);

        var parser = new IgnoreList(path);
        return parser.IsIgnored(fileName);
    }

    public void FindGitRoot(string targetDirectory)
    {
        logger.LogDebug("Finding git root for directory {targetDirectory}", targetDirectory);

        while (!string.IsNullOrWhiteSpace(targetDirectory))
        {
            if (Directory.Exists(Path.Combine(targetDirectory, ".git")))
            {
                GlobalsUtils.GitRootDirectory = targetDirectory;
                return;
            }

            var parentDir = Directory.GetParent(targetDirectory);
            targetDirectory = parentDir != null ? parentDir.FullName : string.Empty;
        }

        GlobalsUtils.GitRootDirectory = string.Empty;
    }

    public async Task GetGitInfoAsync(string targetDirectory, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Getting git info for directory {targetDirectory}", targetDirectory);

        try
        {
            GlobalsUtils.TargetDirectory = targetDirectory;

            using StructuredRepository repository = await Repository.Factory.OpenStructureAsync(targetDirectory, cancellationToken).ConfigureAwait(false);

            if (repository.Head is Branch head)
            {
                GlobalsUtils.GitBranchName = head.Name;

                Commit commit = await head.GetHeadCommitAsync(cancellationToken).ConfigureAwait(false);

                GlobalsUtils.GitHash = (commit.Hash).ToString();
                GlobalsUtils.GitAuthor = (commit.Author).ToString();
                GlobalsUtils.GitCommitter = (commit.Committer).ToString();
                GlobalsUtils.GitSubject = (commit.Subject).ToString();
            }

            GlobalsUtils.IsRepo = true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unable to get git info for directory {targetDirectory}", targetDirectory);
            // Ignore errors (e.g., not a git repository)
        }
    }
}
