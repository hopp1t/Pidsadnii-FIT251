using Xunit;
using task02;

namespace task02tests;

public class StudentServiceTests
{
    private readonly StudentService _service;

    public StudentServiceTests()
    {
        var students = new List<Student>
        {
            new() { Name = "Иван", Faculty = "ФИТ", Grades = new List<int> { 5, 4, 5 } },
            new() { Name = "Анна", Faculty = "ФИТ", Grades = new List<int> { 3, 4, 3 } },
            new() { Name = "Петр", Faculty = "Экономика", Grades = new List<int> { 5, 5, 5 } }
        };
        _service = new StudentService(students);
    }

    [Fact]
    public void GetStudentsByFaculty_ShouldReturnOnlyFITStudents()
    {
        var result = _service.GetStudentsByFaculty("ФИТ").ToList();
        Assert.Equal(2, result.Count);
        Assert.True(result.All(s => s.Faculty == "ФИТ"));
    }

    [Fact]
    public void GetStudentsWithMinAverageGrade_ShouldReturnStudentsWithGrade4OrHigher()
    {
        var result = _service.GetStudentsWithMinAverageGrade(4.0).ToList();
        Assert.Equal(2, result.Count);
        Assert.True(result.All(s => s.Grades.Average() >= 4.0));
    }

    [Fact]
    public void GetStudentsOrderedByName_ShouldReturnAlphabeticalOrder()
    {
        var result = _service.GetStudentsOrderedByName().ToList();
        Assert.Equal("Анна", result[0].Name);
        Assert.Equal("Иван", result[1].Name);
        Assert.Equal("Петр", result[2].Name);
    }

    [Fact]
    public void GroupStudentsByFaculty_ShouldCreateTwoGroups()
    {
        var result = _service.GroupStudentsByFaculty();
        Assert.Equal(2, result.Count);
        Assert.Equal(2, result["ФИТ"].Count());
        Assert.Single(result["Экономика"]);
    }

    [Fact]
    public void GetFacultyWithHighestAverageGrade_ShouldReturnEconomics()
    {
        var result = _service.GetFacultyWithHighestAverageGrade();
        Assert.Equal("Экономика", result);
    }
}
