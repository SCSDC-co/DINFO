using dinfo.core.Utils.Globals;

namespace dinfo.core.Helpers.FilesTools;

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

    public static void GetFileType(string fileName)
    {
        string name = Path.GetFileName(fileName);

        if (name.StartsWith("."))
        {
            return;
        }

        string ext = Path.GetExtension(name);
        if (!string.IsNullOrEmpty(ext))
        {
            GlobalsUtils.FilesTypes.Add(ext);
        }
    }
}
