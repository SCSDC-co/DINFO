using dinfo.core.Utils.Globals;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace dinfo.core.Handlers.ConfigTools;

public class YamlConfigStructure
{
    [YamlMember(Alias = "recursive")]
    public bool Recursive { get; set; } = false;

    [YamlMember(Alias = "verbose")]
    public bool Verbose { get; set; } = false;

    [YamlMember(Alias = "ignore_gitignore")]
    public bool IgnoreGitignore { get; set; } = false;

    [YamlMember(Alias = "no_tui")]
    public bool NoTui { get; set; } = false;

    [YamlMember(Alias = "ignored_files_or_directory")]
    public IgnoreFilesOrDirectory IgnoredFilesOrDirectory { get; set; } = new();
}

public class IgnoreFilesOrDirectory
{
    [YamlMember(Alias = "ignored_files")]
    public List<string> IgnoredFiles { get; set; } = new();

    [YamlMember(Alias = "ignored_directory")]
    public List<string> IgnoredDirectory { get; set; } = new();
}

public static class ConfigHelper
{
    public static void FindConfigFile(string targetDirectory)
    {
        while (!string.IsNullOrEmpty(targetDirectory))
        {
            if ((Directory.Exists(Path.Combine(targetDirectory, ".git"))) ||
                 File.Exists(Path.Combine(targetDirectory, "dinfo.yaml")))
            {
                GlobalsUtils.ConfigFilePath = Path.Combine(targetDirectory, "dinfo.yaml");
                return;
            }

            var parentDir = Directory.GetParent(targetDirectory);
            targetDirectory = parentDir != null ? parentDir.FullName : string.Empty;
        }

        GlobalsUtils.ConfigFilePath = string.Empty;
    }

    public static void DeserializeConfigFile(string configFilePath)
    {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

        var configContent = File.ReadAllText(configFilePath);

        var config = deserializer.Deserialize<YamlConfigStructure>(configContent);

        GlobalsUtils.Recursive = config.Recursive;
        GlobalsUtils.Verbose = config.Verbose;
        GlobalsUtils.IgnoreGitignore = config.IgnoreGitignore;
        GlobalsUtils.NoTui = config.NoTui;

        GlobalsUtils.IgnoredDirectories = config.IgnoredFilesOrDirectory.IgnoredDirectory;
        GlobalsUtils.IgnoredFiles = config.IgnoredFilesOrDirectory.IgnoredFiles;
    }
}