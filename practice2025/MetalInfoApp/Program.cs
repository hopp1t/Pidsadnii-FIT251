using System.Reflection;
using CommandLib;

if (args.Length == 0)
{
    Console.WriteLine("Использование: MetalInfoApp <путь к DLL>");
    return;
}

var path = args[0];
if (!File.Exists(path))
{
    Console.WriteLine($"Файл не найден: {path}");
    return;
}

Console.WriteLine($"Анализ библиотеки: {Path.GetFullPath(path)}\n");
var assembly = Assembly.LoadFrom(Path.GetFullPath(path));

foreach (var type in assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract))
{
    Console.WriteLine($"=== Класс: {type.Name} ===");

    foreach (var attr in type.GetCustomAttributes())
    {
        Console.WriteLine($"  Атрибут: {attr.GetType().Name}");
        if (attr is DisplayNameAttribute d) Console.WriteLine($"    -> DisplayName: {d.DisplayName}");
        if (attr is VersionAttribute v) Console.WriteLine($"    -> Version: {v.Major}.{v.Minor}");
    }

    foreach (var ctor in type.GetConstructors())
    {
        Console.WriteLine($"  Конструктор: {ctor.Name}");
        foreach (var p in ctor.GetParameters())
            Console.WriteLine($"    Параметр: {p.Name} (тип: {p.ParameterType.Name})");
    }

    foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
    {
        if (method.IsSpecialName) continue;
        Console.WriteLine($"  Метод: {method.Name}");
        foreach (var p in method.GetParameters())
            Console.WriteLine($"    Параметр: {p.Name} (тип: {p.ParameterType.Name})");
    }
    Console.WriteLine();
}
