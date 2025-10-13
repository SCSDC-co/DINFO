namespace dinfo.core.Interfaces.Output;

public interface IOutputHandler
{
    Task DirectorySaveAsync(string targetDirectory, string filePath, CancellationToken cancellationToken = default);

    Task FileSaveAsync(string targetDirectory, string filePath, CancellationToken cancellationToken = default);
}
