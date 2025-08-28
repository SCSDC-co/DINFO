using Helpers.FilesTools;
using Utils.Globals;

namespace Helpers.DirTools;

public static class DirectoryHelper
{
    public static void ProcessDirectory(string targetDirectory)
    {
        string[] fileEntries = Directory.GetFiles(targetDirectory);

        foreach (string fileName in fileEntries)
        {
            Console.WriteLine($"Lines of {fileName}: {FilesHelper.CountLines(fileName)}");

            GlobalsUtils.filesProcessed++;
            GlobalsUtils.totalFiles++;
            GlobalsUtils.totalLines += FilesHelper.CountLines(fileName);
        }

        string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
        foreach (string subdirectory in subdirectoryEntries)
        {
            ProcessDirectory(subdirectory);
        }
    }
}
