using Helpers.DirTools;
using Helpers.LineCounter;

namespace dinfo;

public static class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Please provide a file name");
            return;
        }
        else
        {
            foreach (string arg in args)
            {
                if (!File.Exists(arg))
                {
                    Console.WriteLine($"Invalid input, skipping {arg}");
                }
                else
                {
                    var attributes = File.GetAttributes(arg);

                    if (attributes.HasFlag(FileAttributes.Directory))
                    {
                        DirectoryHelper.ProcessDirectory(arg);
                    }
                    else if (attributes.HasFlag(FileAttributes.Normal))
                    {
                        Console.WriteLine($"Lines of {arg}: {LineCounter_Class.CountLines(arg)}");
                    }
                    else
                    {
                        Console.WriteLine($"Invalid input, skipping {arg}");
                    }
                }
            }
        }
    }
}
