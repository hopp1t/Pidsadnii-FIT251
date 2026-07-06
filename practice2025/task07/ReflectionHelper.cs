namespace task07;

using System;
using System.Reflection;
using System.Text;

public static class ReflectionHelper
{
    public static string PrintTypeInfo(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var sb = new StringBuilder();

        var displayNameAttr = type.GetCustomAttribute<DisplayNameAttribute>();
        if (displayNameAttr is not null)
            sb.AppendLine($"Класс: {displayNameAttr.DisplayName}");
        else
            sb.AppendLine($"Класс: {type.Name}");

        var versionAttr = type.GetCustomAttribute<VersionAttribute>();
        if (versionAttr is not null)
            sb.AppendLine($"Версия: {versionAttr}");

        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        if (methods.Length > 0)
        {
            sb.AppendLine("Методы:");
            foreach (var method in methods)
            {
                var attr = method.GetCustomAttribute<DisplayNameAttribute>();
                sb.AppendLine($"  - {attr?.DisplayName ?? method.Name}");
            }
        }

        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        if (properties.Length > 0)
        {
            sb.AppendLine("Свойства:");
            foreach (var prop in properties)
            {
                var attr = prop.GetCustomAttribute<DisplayNameAttribute>();
                sb.AppendLine($"  - {attr?.DisplayName ?? prop.Name}");
            }
        }

        return sb.ToString().TrimEnd();
    }
}
