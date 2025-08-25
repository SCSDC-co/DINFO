using System;
using Helpers.LineCounter;

namespace dinfo;

public static class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Please provide a file name");
            return;
        }
        else
        {
            foreach (string fileName in args)
                Console.WriteLine($"Lines of {fileName}: {LineCounter.CountLines(fileName)}");
        }
    }
}
