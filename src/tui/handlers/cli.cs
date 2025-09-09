using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using dinfo.core.Utils.Globals;
using dinfo.tui.Helpers.Tui;

[Command(Description = "Display information about the specified directory and its contents.")]
public class DinfoCommand : ICommand
{
    [CommandParameter(0, Description = "The Directory to be analyzed.", IsRequired = false)]
    public string TargetDirectory { get; set; } = Directory.GetCurrentDirectory();

    [CommandOption("recursive", 'r', Description = "Recursively list all files and directories.")]
    public bool RecursiveCli { get; set; } = false;

    [CommandOption("verbose", 'v', Description = "Enable verbose output.")]
    public bool VerboseCli { get; set; } = false;

    [CommandOption("ignore-gitignore", 'i', Description = "Ignore .gitignore files.")]
    public bool IgnoreGitIgnoreCli { get; set; } = false;

    public async ValueTask ExecuteAsync(IConsole console)
    {
        GlobalsUtils.Recursive = RecursiveCli;
        GlobalsUtils.Verbose = VerboseCli;
        GlobalsUtils.IgnoreGitignore = IgnoreGitIgnoreCli;

        var dir = TargetDirectory;

        await TuiHelper.PrintDirectoryInfo(dir);
    }
}

[Command("file", Description = "Display information about the specified file.")]
public class FileCommand : ICommand
{
    [CommandParameter(0, Description = "The File to be analyzed.", IsRequired = true)]
    public string[] TargetFile { get; set; } = Array.Empty<string>();

    public async ValueTask ExecuteAsync(IConsole console)
    {
        int numberOfCycles = 0;
        var numberOfFiles = TargetFile.Length;

        foreach (var TargetFile in TargetFile)
        {
            await TuiHelper.PrintFileInfo(TargetFile);

            numberOfCycles++;
        }

        if (numberOfFiles == numberOfCycles)
        {
            TuiHelper.PrintSummary();
        }
    }
}