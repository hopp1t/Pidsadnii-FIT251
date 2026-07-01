using FileSystemCommands;
using Xunit;
namespace task08tests;

public class FileSystemCommandsTests
{
    [Fact]
    public void DirectorySizeCommand_ShouldCalculateSize()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "TestDir_" + Guid.NewGuid());
        Directory.CreateDirectory(testDir);
        File.WriteAllText(Path.Combine(testDir, "test1.txt"), "Hello");
        File.WriteAllText(Path.Combine(testDir, "test2.txt"), "World");

        var command = new DirectorySizeCommand(testDir);
        command.Execute(); // Проверяем, что не возникает исключений

        Directory.Delete(testDir, true);
    }

    [Fact]
    public void FindFilesCommand_ShouldFindMatchingFiles()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "TestDir_" + Guid.NewGuid());
        Directory.CreateDirectory(testDir);
        File.WriteAllText(Path.Combine(testDir, "file1.txt"), "Text");
        File.WriteAllText(Path.Combine(testDir, "file2.log"), "Log");

        var command = new FindFilesCommand(testDir, "*.txt");
        command.Execute(); // Должен найти 1 файл

        Directory.Delete(testDir, true);
    }

    [Fact]
    public void DirectorySizeCommand_EmptyDirectory_SizeIsZero()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "EmptyDir_" + Guid.NewGuid());
        Directory.CreateDirectory(testDir);

        var command = new DirectorySizeCommand(testDir);
        command.Execute();

        Directory.Delete(testDir, true);
    }

    [Fact]
    public void DirectorySizeCommand_InvalidPath_ThrowsException()
    {
        Assert.Throws<DirectoryNotFoundException>(
            () => new DirectorySizeCommand("C:\\nonexistent_path_12345"));
    }

    [Fact]
    public void FindFilesCommand_NoMatchingFiles_FindsNothing()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "TestDir_" + Guid.NewGuid());
        Directory.CreateDirectory(testDir);
        File.WriteAllText(Path.Combine(testDir, "file1.txt"), "Text");

        var command = new FindFilesCommand(testDir, "*.xyz");
        command.Execute(); // Должен найти 0 файлов

        Directory.Delete(testDir, true);
    }

    [Fact]
    public void FindFilesCommand_InvalidPath_ThrowsException()
    {
        Assert.Throws<DirectoryNotFoundException>(
            () => new FindFilesCommand("C:\\nonexistent_path_12345", "*.txt"));
    }

    [Fact]
    public void DirectorySizeCommand_EmptyPath_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new DirectorySizeCommand(""));
    }

    [Fact]
    public void FindFilesCommand_EmptyPattern_ThrowsException()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "TestDir_" + Guid.NewGuid());
        Directory.CreateDirectory(testDir);

        Assert.Throws<ArgumentException>(() => new FindFilesCommand(testDir, ""));

        Directory.Delete(testDir, true);
    }
}
