using dinfo.core.Helpers.FilesTools;
using dinfo.core.Utils.Globals;
using System.Security.AccessControl;
using System.Security.Principal;

namespace dinfo.core.Helpers.DirTools;

public static class DirectoryHelper
{
    public static void ProcessDirectory(string targetDirectory)
    {
        GlobalsUtils.totalDirs++;
        GetDirectorySize(targetDirectory);

        string[] fileEntries = Directory.GetFiles(targetDirectory);

        foreach (string fileName in fileEntries)
        {
            GlobalsUtils.totalFiles++;
            GlobalsUtils.totalLines += FilesHelper.CountLines(fileName);
            GlobalsUtils.Files.Add(fileName);

            FilesHelper.GetFileType(fileName);
        }

        string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);

        if (GlobalsUtils.recursive)
            foreach (string subdirectory in subdirectoryEntries)
            {
                ProcessDirectory(subdirectory);
            }
    }

    public static void GetDirectorySize(string targetDirectory)
    {
        long dirSize =
            Directory.GetFiles(targetDirectory).Sum(f => new FileInfo(f).Length);

        GlobalsUtils.totalSizeB += dirSize;

        GlobalsUtils.totalSizeMB = GlobalsUtils.totalSizeB / 1000000.0;
    }

    public static double SizeToReturn()
    {
        if (GlobalsUtils.totalSizeB >= 1000000.0)
        {
            GlobalsUtils.sizeExtension = "MB";
            return GlobalsUtils.totalSizeMB;
        }
        else if (GlobalsUtils.totalSizeB <= 1000000.0)
        {
            GlobalsUtils.sizeExtension = "B";
            return GlobalsUtils.totalSizeB;
        }
        else
        {
            return 0;
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
