using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using dinfo.core.Helpers.DirTools;
using dinfo.core.Utils.Globals;
using dinfo.core.Utils.Help;
using Spectre.Console;

namespace dinfo.tui;

public static class Program
{
	public static async Task<Panel> BuildGitPanel(string targetDirectory)
	{
		if (GlobalsUtils.IsRepo)
		{

			await dinfo.core.Helpers.GitTools.GitHelper.GetGitInfo(targetDirectory);

			var gitPanel = new Panel(
				$"[bold green]Git Branch Name:[/] {GlobalsUtils.GitBranchName}\n" +
				$"[bold green]Git Hash:[/] {GlobalsUtils.GitHash}\n" +
				$"[bold green]Git Author:[/] {GlobalsUtils.GitAuthor}\n" +
				$"[bold green]Git Committer:[/] {GlobalsUtils.GitCommitter}\n" +
				$"[bold green]Git Subject:[/] {GlobalsUtils.GitSubject}\n"
			);

			gitPanel.Border = BoxBorder.Rounded;
			gitPanel.BorderStyle = new Style(Color.Green);
			gitPanel.Header = new PanelHeader("[bold green] GIT [/]");

			return gitPanel;
		}
		else
		{
			var gitPanel = new Panel("[bold red]Not a git repository[/]");

			gitPanel.Border = BoxBorder.Rounded;
			gitPanel.BorderStyle = new Style(Color.Red);
			gitPanel.Header = new PanelHeader("[bold red] GIT [/]");

			return gitPanel;
		}
	}

	public static async Task PrintDirectoryInfo(string targetDirectory)
	{
		await DirectoryHelper.ProcessDirectory(targetDirectory);

		/*
		 *  HEADER
		 */
		var headerPanel = new Panel($"[bold green]DINFO: {targetDirectory}[/]");

		headerPanel.Border = BoxBorder.Rounded;
		headerPanel.Expand = true;
		headerPanel.BorderStyle = new Style(Color.Green);

		AnsiConsole.Write(headerPanel);

		/*
		 *  INFO
		 */

		var infoPanel = new Panel(
			$"[bold green]Number of files:[/] {GlobalsUtils.TotalFiles}\n" +
			$"[bold green]Number of lines:[/] {GlobalsUtils.TotalLines}\n" +
			$"[bold green]Number of directories:[/] {GlobalsUtils.TotalDirs}\n" +
			$"[bold green]Total size:[/] {DirectoryHelper.SizeToReturn()} {GlobalsUtils.SizeExtension}");

		infoPanel.Border = BoxBorder.Rounded;
		infoPanel.BorderStyle = new Style(Color.Green);
		infoPanel.Header = new PanelHeader("[bold green] INFO [/]");

		/*
		 *  PERIMSSIONS
		 */

		var perms = DirectoryHelper.GetDirectoryPermissions(targetDirectory);
		var permissionPanel = new Panel($"{perms}");

		permissionPanel.Border = BoxBorder.Rounded;
		permissionPanel.BorderStyle = new Style(Color.Green);
		permissionPanel.Header = new PanelHeader("[bold green] PERMISSIONS [/]");

		/*
		 *  FILES EXTENSIONS
		 */

		var fileTypesNoDupes = GlobalsUtils.FileTypes.Distinct().ToList();

		var mostUsedExtension = GlobalsUtils.FileTypes
			.GroupBy(x => x)
			.OrderByDescending(g => g.Count())
			.Select(g => g.Key)
			.FirstOrDefault() ?? "N/A";

		var extensionsPanel = new Panel(
			$"[bold green]File extensions:[/] {string.Join(", ", fileTypesNoDupes)}\n" +
			$"[bold green]Most used extension:[/] {mostUsedExtension.TrimStart('.')}");

		extensionsPanel.Border = BoxBorder.Rounded;
		extensionsPanel.BorderStyle = new Style(Color.Green);
		extensionsPanel.Header = new PanelHeader("[bold green] EXTENSIONS [/]");

		/*
		 *  GRID
		 */

		var infoColumns = new Grid();

		infoColumns.AddColumn();
		infoColumns.AddColumn();
		infoColumns.AddColumn();
		infoColumns.AddColumn();

		infoColumns.AddRow(infoPanel, permissionPanel, extensionsPanel, await BuildGitPanel(targetDirectory));

		AnsiConsole.Write(infoColumns);

		if (GlobalsUtils.Verbose)
		{
			var filesPanel =
				new Panel($"[bold green]{string.Join(", ", GlobalsUtils.Files)}[/] ");

			filesPanel.Border = BoxBorder.Rounded;
			filesPanel.BorderStyle = new Style(Color.Green);
			filesPanel.Header = new PanelHeader("[bold green] FILES [/]");

			AnsiConsole.Write(filesPanel);
		}
	}

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

				case "-h":
				case "--help":
					HelpUtils.PrintHelp();
					return;

				default:
					if (Directory.Exists(arg))
					{
						await PrintDirectoryInfo(arg);
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
			await PrintDirectoryInfo(currentDirectory);
		}
	}
}
