using CommandLib;
namespace FileSystemCommands;

[DisplayName("Вычислить размер каталога")]
[Version(1, 0)]
public class DirectorySizeCommand : ICommand
{
    private readonly string _directoryPath;

    public DirectorySizeCommand(string directoryPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directoryPath);
        if (!Directory.Exists(directoryPath))
            throw new DirectoryNotFoundException($"Каталог не найден: {directoryPath}");

        _directoryPath = directoryPath;
    }

    public void Execute()
    {
        var size = CalculateDirectorySize(new DirectoryInfo(_directoryPath));
        Console.WriteLine($"Размер каталога '{_directoryPath}': {size} байт");
    }

    private static long CalculateDirectorySize(DirectoryInfo directory)
    {
        long size = 0;

        foreach (var file in directory.GetFiles())
        {
            size += file.Length;
        }

        foreach (var subDir in directory.GetDirectories())
        {
            size += CalculateDirectorySize(subDir);
        }

        return size;
    }
}
