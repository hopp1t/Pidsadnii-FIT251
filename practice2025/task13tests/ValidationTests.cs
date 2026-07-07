using task13;
using Xunit;

namespace task13tests;

public class ValidationTests
{
    [Fact]
    public void Validate_ValidStudent_ShouldNotThrow()
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

        var exception = Record.Exception(() => StudentValidator.Validate(student));
        Assert.Null(exception);
    }

    [Fact]
    public void Validate_NullFirstName_ShouldThrow()
    {
        var student = new Student
        {
            FirstName = null,
            LastName = "Петров",
            BirthDate = new DateTime(2000, 5, 15),
            Grades = new List<Subject>()
        };

        var ex = Assert.Throws<InvalidOperationException>(() => StudentValidator.Validate(student));
        Assert.Contains("FirstName", ex.Message);
    }

    [Fact]
    public void Validate_EmptyLastName_ShouldThrow()
    {
        var student = new Student
        {
            FirstName = "Иван",
            LastName = "",
            BirthDate = new DateTime(2000, 5, 15),
            Grades = new List<Subject>()
        };

        var ex = Assert.Throws<InvalidOperationException>(() => StudentValidator.Validate(student));
        Assert.Contains("LastName", ex.Message);
    }

    [Fact]
    public void Validate_FutureBirthDate_ShouldThrow()
    {
        var student = new Student
        {
            FirstName = "Иван",
            LastName = "Петров",
            BirthDate = DateTime.Now.AddYears(1),
            Grades = new List<Subject>()
        };

        var ex = Assert.Throws<InvalidOperationException>(() => StudentValidator.Validate(student));
        Assert.Contains("BirthDate", ex.Message);
    }

    [Fact]
    public void Validate_InvalidGrade_ShouldThrow()
    {
        var student = new Student
        {
            FirstName = "Иван",
            LastName = "Петров",
            BirthDate = new DateTime(2000, 5, 15),
            Grades = new List<Subject>
            {
                new Subject { Name = "Математика", Grade = 10 }
            }
        };

        var ex = Assert.Throws<InvalidOperationException>(() => StudentValidator.Validate(student));
        Assert.Contains("Математика", ex.Message);
        Assert.Contains("2-5", ex.Message);
    }

    [Fact]
    public void Validate_EmptySubjectName_ShouldThrow()
    {
        var student = new Student
        {
            FirstName = "Иван",
            LastName = "Петров",
            BirthDate = new DateTime(2000, 5, 15),
            Grades = new List<Subject>
            {
                new Subject { Name = "", Grade = 5 }
            }
        };

        var ex = Assert.Throws<InvalidOperationException>(() => StudentValidator.Validate(student));
        Assert.Contains("Name", ex.Message);
    }

    [Fact]
public void Deserialize_InvalidStudent_ShouldThrowInvalidOperationException()
{
    var json = """
    {
        "FirstName": "",
        "LastName": "Петров",
        "BirthDate": "2000-05-15T00:00:00",
        "Grades": []
    }
    """;

    Assert.Throws<InvalidOperationException>(() => StudentJsonSerializer.Deserialize(json));
}

    [Fact]
    public void Deserialize_WithInvalidGrade_ShouldThrowInvalidOperationException()
    {
        var json = """
        {
            "FirstName": "Иван",
            "LastName": "Петров",
            "BirthDate": "2000-05-15T00:00:00",
            "Grades": [
                {"Name": "Математика", "Grade": 10}
            ]
        }
        """;

        Assert.Throws<InvalidOperationException>(() => StudentJsonSerializer.Deserialize(json));
    }
}
