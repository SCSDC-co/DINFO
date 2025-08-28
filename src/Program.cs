using Helpers.DirTools;
using Helpers.FilesTools;
using Utils.Globals;

namespace dinfo;

public static class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            DirectoryHelper.ProcessDirectory(Directory.GetCurrentDirectory());
        }
        else
        {
            foreach (string arg in args)
            {
                if (!File.Exists(arg) && !Directory.Exists(arg))
                    Console.WriteLine($"Invalid input, skipping {arg}");

                var attributes = File.GetAttributes(arg);

                if (attributes.HasFlag(FileAttributes.Directory))
                {
                    DirectoryHelper.ProcessDirectory(arg);
                }
                else if (attributes.HasFlag(FileAttributes.Normal))
                {
                    Console.WriteLine($"Lines of {arg}: {FilesHelper.CountLines(arg)}");

                    GlobalsUtils.filesProcessed++;
                    GlobalsUtils.totalFiles++;
                    GlobalsUtils.totalLines += FilesHelper.CountLines(arg);
                }
                else
                {
                    Console.WriteLine($"Invalid input, skipping {arg}");
                }
            }
        }
        Console.WriteLine();
        Console.WriteLine(new string('-', 20));
        Console.WriteLine();

        Console.WriteLine($"Total files: {GlobalsUtils.totalFiles}");
        Console.WriteLine($"Files processed: {GlobalsUtils.filesProcessed}");
        Console.WriteLine($"Total lines: {GlobalsUtils.totalLines}");
    }
}
