using System;
using System.IO;
using System.Linq;

namespace Helpers.LineCounter;

public static class LineCounter
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
