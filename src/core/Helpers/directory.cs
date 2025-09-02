using dinfo.core.Helpers.FilesTools;
using dinfo.core.Utils.Globals;

namespace dinfo.core.Helpers.DirTools;

public static class DirectoryHelper
{
    public static void ProcessDirectory(string targetDirectory)
    {
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
                GlobalsUtils.totalDirs++;
            }
    }
}
