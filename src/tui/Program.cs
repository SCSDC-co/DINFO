using dinfo.core.Helpers.DirTools;
using dinfo.core.Utils.Globals;
using dinfo.core.Utils.Help;
using Spectre.Console;

namespace dinfo.tui;

public static class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            var currentDirectory = Directory.GetCurrentDirectory();

            DirectoryHelper.ProcessDirectory(currentDirectory);

            var headerPanel = new Panel($"[bold green]DINFO: {currentDirectory}[/]");

            headerPanel.Border = BoxBorder.Rounded;
            headerPanel.Padding = new Padding(1, 1, 1, 1);
            headerPanel.Expand = true;
            headerPanel.BorderStyle = new Style(Color.Green);

            AnsiConsole.Write(headerPanel);
        }

        if (args.Length == 1 && (args[0] == "-h" || args[0] == "--help"))
        {
            HelpUtils.PrintHelp();
            return;
        }

        foreach (string arg in args)
        {
            var attributes = File.GetAttributes(arg);

            if (!Directory.Exists(arg))
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
    }
}
