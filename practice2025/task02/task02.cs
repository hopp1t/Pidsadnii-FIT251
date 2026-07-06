using System.Linq;

namespace task02;

public class StudentService
{
    private readonly List<Student> _students;

    public StudentService(List<Student> students)
    {
        ArgumentNullException.ThrowIfNull(students);
        _students = students;
    }

    public IEnumerable<Student> GetStudentsByFaculty(string faculty)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(faculty);
        return _students.Where(s => s.Faculty == faculty);
    }

    public IEnumerable<Student> GetStudentsWithMinAverageGrade(double minAverageGrade)
        => _students.Where(s => s.Grades.Count > 0 && s.Grades.Average() >= minAverageGrade);

    public IEnumerable<Student> GetStudentsOrderedByName()
        => _students.OrderBy(s => s.Name);

    public ILookup<string, Student> GroupStudentsByFaculty()
        => _students.ToLookup(s => s.Faculty);

    public string GetFacultyWithHighestAverageGrade()
    {
        var topFaculty = _students
            .Where(s => s.Grades.Count > 0)
            .GroupBy(s => s.Faculty)
            .OrderByDescending(g => g.Average(s => s.Grades.Average()))
            .FirstOrDefault();

        return topFaculty?.Key ?? string.Empty;
    }
}
