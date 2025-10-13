namespace dinfo.core.Handlers.Json;

public class DirectoryJson
{
    public string Path { get; set; } = "N/A";
    public string BiggestFile { get; set; } = "N/A";
    public string LatestModifiedFile { get; set; } = "N/A";
    public string Size { get; set; } = "N/A";
    public string Permissions { get; set; } = "N/A";
    public string MostUsedExtension { get; set; } = "N/A";
    public List<string> FileType { get; set; } = ["N/A"];
    public List<string> Encoding { get; set; } = ["N/A"];
    public int Files { get; set; }
    public int Directories { get; set; }
    public int Comments { get; set; }
    public int Blank { get; set; }
    public int Code { get; set; }
    public int Lines { get; set; }
    public required Dictionary<string, GitJson> Git { get; set; }
}
