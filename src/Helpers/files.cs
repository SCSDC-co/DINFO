namespace Helpers.FilesTools;

public static class FilesHelper
{
    public static int CountLines(string fileName)
    {
        if (!File.Exists(fileName))
        {
            throw new FileNotFoundException($"File not found: {fileName}");
        }

        IEnumerable<string> lines = File.ReadLines(fileName);

        return lines.Count();
    }
}
