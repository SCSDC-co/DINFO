namespace Utils.Help;

public static class HelpUtils
{
    public static void PrintHelp()
    {
        Console.WriteLine("Usage: dinfo [options] [directory|file1 file2]");
        Console.WriteLine();
        Console.WriteLine("Description:");
        Console.WriteLine(
            "   dinfo is a simple tool to count the number of lines in a directory or a file."
        );
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("   -h, --help          Print this help message.");
        Console.WriteLine("   -r, --recursive     Recursively process subdirectories.");
    }
}
