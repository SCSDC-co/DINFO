using Helpers.LineCounter;

namespace Helpers.DirTools;

public static class DirectoryHelper
{
    public static void ProcessDirectory(string targetDirectory)
    {
        string[] fileEntries = Directory.GetFiles(targetDirectory);

        foreach (string fileName in fileEntries)
        {
            Console.WriteLine($"Lines of {fileName}: {LineCounter_Class.CountLines(fileName)}");
        }

        string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
        foreach (string subdirectory in subdirectoryEntries)
        {
            ProcessDirectory(subdirectory);
        }
    }
}
