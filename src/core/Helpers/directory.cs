using dinfo.core.Helpers.FilesTools;
using dinfo.core.Utils.Globals;
using System.Security.AccessControl;
using System.Security.Principal;

namespace dinfo.core.Helpers.DirTools;

public static class DirectoryHelper
{
    public async static Task ProcessDirectory(string targetDirectory)
    {
        GlobalsUtils.TotalDirs++;
        GetDirectorySize(targetDirectory);

        string[] fileEntries = Directory.GetFiles(targetDirectory);

        foreach (string fileName in fileEntries)
        {
            GlobalsUtils.TotalFiles++;
            GlobalsUtils.TotalLines += await FilesHelper.CountLines(fileName);
            GlobalsUtils.Files.Add(fileName);

            FilesHelper.GetFileType(fileName);
        }

        string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);

        if (GlobalsUtils.Recursive)
        {
            foreach (string subdirectory in subdirectoryEntries)
            {
                await ProcessDirectory(subdirectory);
            }
        }
    }

    public static void GetDirectorySize(string targetDirectory)
    {
        long dirSize =
            Directory.GetFiles(targetDirectory).Sum(f => new FileInfo(f).Length);

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
