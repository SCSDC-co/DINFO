using dinfo.core.Helpers.DirTools;
using dinfo.core.Utils.Globals;
using dinfo.core.Utils.Help;

namespace dinfo.tui;

public static class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            DirectoryHelper.ProcessDirectory(Directory.GetCurrentDirectory());
        }

        if (args.Length == 1 && (args[0] == "-h" || args[0] == "--help"))
        {
            HelpUtils.PrintHelp();
            return;
        }

        foreach (string arg in args)
        {
            var attributes = File.GetAttributes(arg);

            if (!File.Exists(arg) && !Directory.Exists(arg))
            {
                Console.WriteLine($"Invalid input, skipping {arg}");
                continue;
            }

            if (arg == "-r" || arg == "--recursive")
            {
                GlobalsUtils.recursive = true;
                continue;
            }

            if (attributes.HasFlag(FileAttributes.Directory))
            {
                DirectoryHelper.ProcessDirectory(arg);
            }
            else
            {
                Console.WriteLine($"Invalid input, skipping {arg}");
            }
        }

        Console.WriteLine();
        Console.WriteLine(new string('-', 13));
        Console.WriteLine("   SUMMARY");
        Console.WriteLine(new string('-', 13));
        Console.WriteLine();

        Console.WriteLine($"Total files: {GlobalsUtils.totalFiles}");
        Console.WriteLine($"Total lines: {GlobalsUtils.totalLines}");
    }
}
