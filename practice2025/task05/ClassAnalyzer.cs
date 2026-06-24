using System.Reflection;

namespace task05;

public class ClassAnalyzer
{
    private readonly Type _type;

    public ClassAnalyzer(Type type)
    {
        _type = type ?? throw new ArgumentNullException(nameof(type));
    }

    public IEnumerable<string> GetPublicMethods()
        => _type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(m => m.Name);

    public IEnumerable<string> GetMethodParams(string methodName)
    {
        var method = _type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            ?? throw new ArgumentException($"Метод '{methodName}' не найден в классе '{_type.Name}'.");

        var parameters = method.GetParameters()
                               .Select(p => $"{p.ParameterType.Name} {p.Name}");

        var returnType = new[] { $"Returns: {method.ReturnType.Name}" };

        return parameters.Concat(returnType);
    }

    public IEnumerable<string> GetAllFields()
        => _type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                .Select(f => f.Name);

    public IEnumerable<string> GetProperties()
        => _type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                .Select(p => p.Name);

    public bool HasAttribute<T>() where T : Attribute
        => _type.GetCustomAttributes(typeof(T), true).Any();
}
