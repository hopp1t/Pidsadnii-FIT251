using System.Text.Json;
using System.Text.Json.Serialization;

namespace task13;

public static class StudentJsonSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = null, 
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping // для кириллицы
    };

    public static string Serialize(Student student)
    {
        ArgumentNullException.ThrowIfNull(student);
        return JsonSerializer.Serialize(student, Options);
    }

    public static Student Deserialize(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);
        return JsonSerializer.Deserialize<Student>(json, Options)
            ?? throw new JsonException("Десериализация вернула null");
    }
}
