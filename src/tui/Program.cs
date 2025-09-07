using dinfo.tui.Helpers.tui;
using dinfo.core.Utils.Globals;

namespace dinfo.tui;

public static class Program
{
	public static async Task Main(string[] args)
	{
		var currentDirectory = Directory.GetCurrentDirectory();
		bool hasDirectory = false;

		foreach (string arg in args)
		{
			switch (arg)
			{
				case "-r":
				case "--Recursive":
					GlobalsUtils.Recursive = true;
					break;

				case "-v":
				case "--Verbose":
					GlobalsUtils.Verbose = true;
					break;

				case "-ig":
				case "--Ignore-Gitignore":
					GlobalsUtils.IgnoreGitignore = true;
					break;

				case "-h":
				case "--help":
					TuiHelper.PrintHelp();
					return;

				default:
					if (Directory.Exists(arg))
					{
						await TuiHelper.PrintDirectoryInfo(arg);
						hasDirectory = true;
					}
					else
					{
						Console.WriteLine($"Invalid input, skipping {arg}");
					}
					break;
			}
		}

		if (!hasDirectory)
		{
			await TuiHelper.PrintDirectoryInfo(currentDirectory);
		}
	}
}
