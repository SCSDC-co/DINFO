using System.Globalization;
using dinfo.core.Helpers.FilesTools;
using dinfo.core.Utils.Globals;

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
        long dirSize = Directory.GetFiles(targetDirectory)
                                .Sum(f => new FileInfo(f).Length);

        GlobalsUtils.totalSizeB += dirSize;

        GlobalsUtils.totalSizeMB = GlobalsUtils.totalSizeB / 1000000.0;
    }

    public static double sizeToReturn()
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
}
