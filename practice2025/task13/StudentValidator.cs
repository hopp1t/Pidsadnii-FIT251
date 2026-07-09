namespace task13;

public static class StudentValidator
{
    public static void Validate(Student student)
    {
        ArgumentNullException.ThrowIfNull(student);

        if (string.IsNullOrWhiteSpace(student.FirstName))
            throw new InvalidOperationException("Поле FirstName не может быть пустым");

        if (string.IsNullOrWhiteSpace(student.LastName))
            throw new InvalidOperationException("Поле LastName не может быть пустым");

        if (student.BirthDate > DateTime.Now)
            throw new InvalidOperationException("Поле BirthDate не может быть в будущем");

        if (student.BirthDate < new DateTime(1900, 1, 1))
            throw new InvalidOperationException("Поле BirthDate слишком старое");

        if (student.Grades != null)
        {
            foreach (var subject in student.Grades)
            {
                if (string.IsNullOrWhiteSpace(subject.Name))
                    throw new InvalidOperationException("Название предмета (Name) не может быть пустым");

                if (subject.Grade < 2 || subject.Grade > 5)
                    throw new InvalidOperationException(
                        $"Оценка по предмету '{subject.Name}' должна быть в диапазоне 2-5, получено: {subject.Grade}");
            }
        }
    }
}
