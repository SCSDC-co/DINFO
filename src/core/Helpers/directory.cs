using dinfo.core.Handlers.ConfigTools;
using dinfo.core.Helpers.FilesTools;
using dinfo.core.Helpers.GitTools;
using dinfo.core.Utils.Globals;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace dinfo.core.Helpers.DirTools;

public static class DirectoryHelper
{
    public static async Task ProcessDirectoryAsync(string targetDirectory, CancellationToken cancellationToken = default)
    {
        GlobalsUtils.TotalDirs++;
        GetDirectorySize(targetDirectory);

        string[] fileEntries = Directory.GetFiles(targetDirectory);

        GitHelper.FindGitRoot(targetDirectory);

        var gitIgnorePath = Path.Combine(
            GlobalsUtils.GitRootDirectory.Replace("\\", "/"),
            ".gitignore"
        );

        ConfigHelper.FindConfigFile(targetDirectory);

        ConfigHelper.DeserializeConfigFile(GlobalsUtils.ConfigFilePath);

        foreach (string fileName in fileEntries)
        {
            var fileInfo = new FileInfo(fileName);

            string relativePath = string.IsNullOrEmpty(GlobalsUtils.TargetDirectory)
                ? ""
                : Path.GetRelativePath(GlobalsUtils.TargetDirectory, fileName);

            bool isIgnored = GlobalsUtils.IgnoredFiles.Any(pattern =>
            {
                string regexPattern = "^" + Regex.Escape(pattern).Replace("\\*", ".*") + "$";
                return Regex.IsMatch(fileName, regexPattern, RegexOptions.IgnoreCase);
            });

            if (isIgnored)
            {
                continue;
            }

            if (!GlobalsUtils.IgnoreGitignore && File.Exists(gitIgnorePath))
            {
                var isIgnoredGit = GitHelper.IsFileIgnore(gitIgnorePath, fileInfo);

                if (isIgnoredGit)
                {
                    continue;
                }
            }

            try
            {
                GlobalsUtils.TotalFiles++;
                GlobalsUtils.TotalLines += await FilesHelper.CountLinesAsync(fileName, cancellationToken).ConfigureAwait(false);
                GlobalsUtils.Files.Add(fileName);

                var encoding = await FilesHelper.GetEncodingAsync(fileName, cancellationToken).ConfigureAwait(false);
                GlobalsUtils.Encodings.Add(encoding.WebName);

                FilesHelper.GetFileType(fileName);
                await FilesHelper.GetCommentsLinesAsync(fileName, cancellationToken).ConfigureAwait(false);
                await FilesHelper.GetBlankLinesAsync(fileName, cancellationToken).ConfigureAwait(false);
            }
            catch (IOException)
            {
                GlobalsUtils.SkippedFileLocked.Add(fileName);
                continue;
            }
            catch (UnauthorizedAccessException)
            {
                GlobalsUtils.SkippedFileAccesDenied.Add(fileName);
                continue;
            }
        }

        FilesHelper.GetLastModifiedFile(targetDirectory);

        string[] subDirectoryEntries = Directory.GetDirectories(targetDirectory);

        if (GlobalsUtils.Recursive)
        {
            foreach (string subDirectory in subDirectoryEntries)
            {
                if (
                    Path.GetFileName(subDirectory)
                        .Equals(".git", StringComparison.OrdinalIgnoreCase)
                )
                {
                    continue;
                }

                string relativePath = string.IsNullOrEmpty(GlobalsUtils.TargetDirectory)
                    ? ""
                    : Path.GetRelativePath(GlobalsUtils.TargetDirectory, subDirectory);

                bool isIgnoredDir = GlobalsUtils.IgnoredDirectories.Any(pattern =>
                {
                    string regexPattern = "^" + Regex.Escape(pattern).Replace("\\*", ".*") + "$";
                    string dirName = new DirectoryInfo(subDirectory).Name;
                    return Regex.IsMatch(dirName, regexPattern, RegexOptions.IgnoreCase);
                });

                if (isIgnoredDir)
                {
                    continue;
                }

                await ProcessDirectoryAsync(subDirectory, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    public static void GetDirectorySize(string targetDirectory)
    {
        long dirSize = Directory.GetFiles(targetDirectory).Sum(f => new FileInfo(f).Length);

        var files = Directory.GetFiles(targetDirectory);

        if (files.Length == 0)
        {
            GlobalsUtils.BiggestFile = "N/A";
            GlobalsUtils.BiggestFileSize = 0;
        }
        else
        {
            var biggestFile = files
                .Select(f => new FileInfo(f))
                .OrderByDescending(f => f.Length)
                .First();

            GlobalsUtils.BiggestFile = biggestFile.FullName;
            GlobalsUtils.BiggestFileSize = biggestFile.Length;
        }

        GlobalsUtils.TotalSizeB += dirSize;

        GlobalsUtils.TotalSizeKB = GlobalsUtils.TotalSizeB / 1024.0;
        GlobalsUtils.TotalSizeMB = GlobalsUtils.TotalSizeB / 1024.0 / 1024.0;
        GlobalsUtils.TotalSizeGB = GlobalsUtils.TotalSizeB / 1024.0 / 1024.0 / 1024.0;
    }

    public static double SizeToReturn()
    {
        switch (GlobalsUtils.TotalSizeB)
        {
            case >= 1000000000L:
                GlobalsUtils.SizeExtension = "GB";
                return GlobalsUtils.TotalSizeGB;

            case >= 1000000L:
                GlobalsUtils.SizeExtension = "MB";
                return GlobalsUtils.TotalSizeMB;

            case >= 1000L:
                GlobalsUtils.SizeExtension = "KB";
                return GlobalsUtils.TotalSizeKB;

            default:
                GlobalsUtils.SizeExtension = "B";
                return GlobalsUtils.TotalSizeB;
        }
    }

    public static string GetDirectoryPermissions(string path)
    {
        if (!Directory.Exists(path))
            return "Directory does not exist";

        if (OperatingSystem.IsWindows())
        {
            try
            {
                var dirInfo = new DirectoryInfo(path);
                var acl = dirInfo.GetAccessControl();
                var rules = acl.GetAccessRules(true, true, typeof(NTAccount));

                string result = "";
                foreach (FileSystemAccessRule rule in rules)
                {
                    result +=
                        $"{rule.IdentityReference}: {rule.AccessControlType} {rule.FileSystemRights}\n";
                }

                return result.Trim();
            }
            catch (Exception ex)
            {
                return $"Error reading permissions: {ex.Message}";
            }
        }
        else
        {
            try
            {
#if NET7_0_OR_GREATER
                var unixMode = File.GetUnixFileMode(path);
                return unixMode.ToString();
#else
                return "Unix permissions not available on this .NET version";
#endif
            }
            catch (Exception ex)
            {
                return $"Error reading Unix permissions: {ex.Message}";
            }
        }
    }
}
