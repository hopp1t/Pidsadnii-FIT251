using System.Reflection;
using CommandLib;

// Проверяем аргументы командной строки
if (args.Length < 1)
{
    Console.WriteLine("Использование: CommandRunner <путь к DLL>");
    Console.WriteLine("Пример: CommandRunner FileSystemCommands.dll");
    return;
}

var dllPath = args[0];

if (!File.Exists(dllPath))
{
    Console.WriteLine($"Ошибка: файл '{dllPath}' не найден.");
    return;
}

Console.WriteLine($"Загрузка библиотеки: {Path.GetFullPath(dllPath)}");

// Динамическая загрузка сборки
Assembly assembly;
try
{
    assembly = Assembly.LoadFrom(dllPath);
}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка загрузки библиотеки: {ex.Message}");
    return;
}

// Находим все типы, реализующие ICommand
var commandTypes = assembly.GetTypes()
    .Where(t => typeof(ICommand).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

if (!commandTypes.Any())
{
    Console.WriteLine("Команды не найдены.");
    return;
}

foreach (var commandType in commandTypes)
{
    Console.WriteLine($"Найдена команда: {commandType.Name}");

    try
    {
        ICommand? command = null;

        // Создаём экземпляр в зависимости от конструктора
        if (commandType.Name == "DirectorySizeCommand")
        {
            var testDir = Path.GetTempPath();
            command = (ICommand)Activator.CreateInstance(commandType, testDir)!;
        }
        else if (commandType.Name == "FindFilesCommand")
        {
            var testDir = Path.GetTempPath();
            command = (ICommand)Activator.CreateInstance(commandType, testDir, "*.dll")!;
        }
        else
        {
            command = (ICommand?)Activator.CreateInstance(commandType);
        }

        if (command is not null)
        {
            Console.WriteLine("Выполнение команды:");
            command.Execute();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка при выполнении {commandType.Name}: {ex.Message}");
    }

    Console.WriteLine();
}
