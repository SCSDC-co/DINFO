using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using dinfo.core.Utils.Globals;
using dinfo.tui.Helpers.Tui;

[Command(Description = "Display information about the specified directory and its contents.")]
public class DinfoCommand : ICommand
{
    [CommandParameter(0, Description = "The Directory to be analyzed.")]
    public string Directory { get; set; } = ".";

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

        var dir = Directory;

        await TuiHelper.PrintDirectoryInfo(dir);
    }
}
