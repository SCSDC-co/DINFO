using System.Runtime.Serialization;
using dinfo.core.Utils.Globals;
using Tomlyn;

namespace dinfo.core.Handlers.TomlConfiguration;

public class ConfigFile
{
    [DataMember(Name = "Config")]
    public ConfigSection Config { get; set; } = new();

    [DataMember(Name = "Ignore")]
    public IgnoreSection Ignore { get; set; } = new();
}

public class ConfigSection
{
    public bool RecursiveConfig { get; set; } = false;
    public bool GitignoreConfig { get; set; } = false;
    public bool VerboseConfig { get; set; } = false;
    public bool NoTuiConfig { get; set; } = false;
    public bool IgnoreGitignoreConfig { get; set; } = false;
}

public class IgnoreSection
{
    [DataMember(Name = "DirectoryPaths")]
    public List<string> DirectoryPaths { get; set; } = new();

    [DataMember(Name = "FilePaths")]
    public List<string> FilePaths { get; set; } = new();
}

public class ConfigFileHandler
{
    public static void FindTomlRoot(string targetDirectory)
    {
        while (!string.IsNullOrEmpty(targetDirectory))
        {
            if (Directory.Exists(Path.Combine(targetDirectory, ".git")))
            {
                GlobalsUtils.ConfigFilePath = targetDirectory;
                return;
            }

            var parentDir = Directory.GetParent(targetDirectory);
            targetDirectory = parentDir != null ? parentDir.FullName : string.Empty;
        }

        GlobalsUtils.ConfigFilePath = string.Empty;
    }

    public static void DeserializeConfigFile(string configFilePath)
    {
        string configContent = File.ReadAllText(configFilePath);

        var config = Toml.ToModel<ConfigFile>(configContent);

        GlobalsUtils.Recursive = config.Config.RecursiveConfig;
        GlobalsUtils.IgnoreGitignore = config.Config.IgnoreGitignoreConfig;
        GlobalsUtils.NoTui = config.Config.NoTuiConfig;
        GlobalsUtils.Verbose = config.Config.VerboseConfig;

        GlobalsUtils.IgnoredFiles = config.Ignore.FilePaths;
        GlobalsUtils.IgnoredDirectories = config.Ignore.DirectoryPaths;
    }
}