using task13;
using Xunit;

namespace task13tests;

public class FileOperationTests : IDisposable
{
    private readonly string _testFilePath;

    public FileOperationTests()
    {
        _testFilePath = Path.Combine(Path.GetTempPath(), $"test_student_{Guid.NewGuid()}.json");
    }

    public void Dispose()
    {
        if (File.Exists(_testFilePath))
            File.Delete(_testFilePath);
    }

    [Fact]
    public void SaveToFile_ShouldCreateJsonFile()
    {
        var student = new Student
        {
            FirstName = "Иван",
            LastName = "Петров",
            BirthDate = new DateTime(2000, 5, 15),
            Grades = new List<Subject>
            {
                new Subject { Name = "Математика", Grade = 5 }
            }
        };

        StudentJsonSerializer.SaveToFile(student, _testFilePath);

        Assert.True(File.Exists(_testFilePath));
        var content = File.ReadAllText(_testFilePath);
        Assert.Contains("Иван", content);
    }

    [Fact]
    public void LoadFromFile_ShouldReturnStudent()
    {
        var student = new Student
        {
            FirstName = "Анна",
            LastName = "Сидорова",
            BirthDate = new DateTime(2001, 3, 20),
            Grades = new List<Subject>
            {
                new Subject { Name = "Информатика", Grade = 5 }
            }
        };

        StudentJsonSerializer.SaveToFile(student, _testFilePath);
        var loaded = StudentJsonSerializer.LoadFromFile(_testFilePath);

        Assert.NotNull(loaded);
        Assert.Equal("Анна", loaded.FirstName);
        Assert.Equal("Сидорова", loaded.LastName);
        Assert.Equal(new DateTime(2001, 3, 20), loaded.BirthDate);
        Assert.Single(loaded.Grades!);
    }

    [Fact]
    public void LoadFromFile_NonExistentFile_ShouldThrowFileNotFoundException()
    {
        Assert.Throws<FileNotFoundException>(
            () => StudentJsonSerializer.LoadFromFile("C:\\nonexistent_file.json"));
    }
}
