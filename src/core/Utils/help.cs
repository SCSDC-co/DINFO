namespace dinfo.core.Utils.Help;

public static class HelpUtils
{
    public static void PrintHelp()
    {
        Console.WriteLine("Usage: dinfo [options] [directory]");
        Console.WriteLine();
        Console.WriteLine("Description:");
        Console.WriteLine(
            "   dinfo is a simple tool to count the number of lines of files in a directory."
        );
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("   -h, --help          Print this help message.");
        Console.WriteLine("   -r, --recursive     Recursively process subdirectories.");
        Console.WriteLine("   -v, --verbose       Print verbose information.");
    }
}
