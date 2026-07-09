using System.Text.Json;
using task13;
using Xunit;

namespace task13tests;

public class SerializationTests
{
    [Fact]
    public void Serialize_Student_ShouldReturnValidJson()
    {
        var student = new Student
        {
            FirstName = "Иван",
            LastName = "Петров",
            BirthDate = new DateTime(2000, 5, 15),
            Grades = new List<Subject>
            {
                new Subject { Name = "Математика", Grade = 5 },
                new Subject { Name = "Физика", Grade = 4 }
            }
        };

        var json = StudentJsonSerializer.Serialize(student);

        Assert.NotNull(json);
        Assert.Contains("Иван", json);
        Assert.Contains("Петров", json);
        Assert.Contains("Математика", json);
    }

    [Fact]
    public void Serialize_StudentWithNullValues_ShouldIgnoreNulls()
    {
        var student = new Student
        {
            FirstName = "Иван",
            LastName = null,
            BirthDate = new DateTime(2000, 5, 15),
            Grades = null
        };

        var json = StudentJsonSerializer.Serialize(student);

        Assert.DoesNotContain("LastName", json);
        Assert.DoesNotContain("Grades", json);
        Assert.Contains("Иван", json);
    }

    [Fact]
    public void Serialize_Student_ShouldFormatDateAsIso8601()
    {
        var student = new Student
        {
            FirstName = "Иван",
            LastName = "Петров",
            BirthDate = new DateTime(2000, 5, 15),
            Grades = new List<Subject>()
        };

        var json = StudentJsonSerializer.Serialize(student);

        Assert.Contains("2000-05-15", json);
    }

    [Fact]
    public void Deserialize_ValidJson_ShouldReturnStudent()
    {
        var json = """
        {
            "FirstName": "Иван",
            "LastName": "Петров",
            "BirthDate": "2000-05-15T00:00:00",
            "Grades": [
                {"Name": "Математика", "Grade": 5},
                {"Name": "Физика", "Grade": 4}
            ]
        }
        """;

        var student = StudentJsonSerializer.Deserialize(json);

        Assert.NotNull(student);
        Assert.Equal("Иван", student.FirstName);
        Assert.Equal("Петров", student.LastName);
        Assert.Equal(new DateTime(2000, 5, 15), student.BirthDate);
        Assert.Equal(2, student.Grades!.Count);
        Assert.Equal("Математика", student.Grades[0].Name);
        Assert.Equal(5, student.Grades[0].Grade);
    }

    [Fact]
    public void Deserialize_InvalidJson_ShouldThrowJsonException()
    {
        var invalidJson = "{ invalid json }";

        Assert.Throws<JsonException>(() => StudentJsonSerializer.Deserialize(invalidJson));
    }
}
