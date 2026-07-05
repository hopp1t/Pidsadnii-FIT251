using CommandLib;
namespace FileSystemCommands;

public class FindFilesCommand : ICommand
{
    private readonly string _directoryPath;
    private readonly string _pattern;

    public FindFilesCommand(string directoryPath, string pattern)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directoryPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(pattern);
        if (!Directory.Exists(directoryPath))
            throw new DirectoryNotFoundException($"Каталог не найден: {directoryPath}");

        _directoryPath = directoryPath;
        _pattern = pattern;
    }

    public void Execute()
    {
        var files = Directory.GetFiles(_directoryPath, _pattern, SearchOption.AllDirectories);
        Console.WriteLine($"Найдено файлов по маске '{_pattern}' в '{_directoryPath}': {files.Length}");
        foreach (var file in files)
        {
            Console.WriteLine($"  - {file}");
        }
    }
}
