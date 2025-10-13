namespace dinfo.core.Handlers.Json;

public class FileJson
{
    public string FileName { get; set; } = "N/A";
    public int Lines { get; set; } = 0;
    public int Comments { get; set; } = 0;
    public int Blanks { get; set; } = 0;
    public int Code { get; set; } = 0;
    public List<string> Encoding { get; set; } = ["N/A"];
    public string FileType { get; set; } = "N/A";
}

