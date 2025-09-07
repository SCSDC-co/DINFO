namespace dinfo.core.Utils.Globals;

public static class GlobalsUtils
{
    public static string SizeExtension = "";
    public static int TotalLines = 0;
    public static int TotalFiles = 0;
    public static int TotalDirs = 0;
    public static long TotalSizeB = 0;
    public static double TotalSizeKB = 0;
    public static double TotalSizeMB = 0;
    public static double TotalSizeGB = 0;
    public static bool Recursive = false;
    public static bool Verbose = false;
    public static bool IsRepo = false;
    public static List<string> Files = [];
    public static List<string> FileTypes = [];
    public static string GitBranchName = "N/A";
    public static string GitHash = "N/A";
    public static string GitAuthor = "N/A";
    public static string GitCommitter = "N/A";
    public static string GitSubject = "N/A";
    public static string TargetDirectory = "";
}
