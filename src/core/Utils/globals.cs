namespace dinfo.core.Utils.Globals;

public static class GlobalsUtils
{
    public static string SizeExtension { get; set; } = string.Empty;
    public static int TotalLines { get; set; } = 0;
    public static int TotalLinesComments { get; set; } = 0;
    public static int TotalBlankLines { get; set; } = 0;
    public static int TotalLinesCode => TotalLines - (TotalLinesComments + TotalBlankLines);
    public static int TotalFiles { get; set; } = 0;
    public static int TotalDirs { get; set; } = 0;
    public static long TotalSizeB { get; set; } = 0;
    public static double TotalSizeKB { get; set; } = 0;
    public static double TotalSizeMB { get; set; } = 0;
    public static double TotalSizeGB { get; set; } = 0;
    public static string BiggestFile { get; set; } = "N/A";
    public static long BiggestFileSize { get; set; } = 0;
    public static bool Recursive { get; set; } = false;
    public static bool Verbose { get; set; } = false;
    public static bool IsRepo { get; set; } = false;
    public static bool IgnoreGitignore { get; set; } = false;
    public static bool NoTui { get; set; } = false;
    public static List<string> Files { get; set; } = [];
    public static List<string> FileTypes { get; set; } = [];
    public static List<string> Encodings { get; set; } = [];
    public static List<string> IgnoredDirectories { get; set; } = [];
    public static List<string> IgnoredFiles { get; set; } = [];
    public static List<string> SkippedFileLocked { get; set; } = [];
    public static List<string> SkippedFileAccesDenied { get; set; } = [];
    public static string GitBranchName { get; set; } = "N/A";
    public static string GitHash { get; set; } = "N/A";
    public static string GitAuthor { get; set; } = "N/A";
    public static string GitCommitter { get; set; } = "N/A";
    public static string GitSubject { get; set; } = "N/A";
    public static string TargetDirectory { get; set; } = "";
    public static string LastModifiedFile { get; set; } = "N/A";
    public static string GitRootDirectory { get; set; } = string.Empty;
    public static string ConfigFilePath { get; set; } = string.Empty;

    public static string GetMostUsedExtension()
    {
        return FileTypes
            .GroupBy(x => x)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefault("N/A");
    }

    public static int GetLinesOfCode()
        => TotalLines - (TotalLinesComments + TotalBlankLines);
}
