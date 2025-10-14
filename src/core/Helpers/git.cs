using dinfo.core.Utils.Globals;
using GitReader;
using GitReader.Structures;
using MAB.DotIgnore;

namespace dinfo.core.Helpers.GitTools;

public static class GitHelper
{
    public static bool IsFileIgnore(string path, FileInfo fileName)
    {
        var parser = new IgnoreList(path);

        return parser.IsIgnored(fileName);
    }

    public static void FindGitRoot(string targetDirectory)
    {
        while (!string.IsNullOrEmpty(targetDirectory))
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

    public static async Task GetGitInfoAsync(string targetDirectory, CancellationToken cancellationToken = default)
    {
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
        catch (Exception)
        {
            // Ignore errors (e.g., not a git repository)
        }
    }
}
