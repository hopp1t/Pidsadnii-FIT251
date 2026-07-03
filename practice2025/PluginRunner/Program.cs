using PluginLoader;

if (args.Length == 0)
{
    Console.WriteLine("Использование: PluginRunner <папка с плагинами>");
    Console.WriteLine("Пример: PluginRunner ./plugins_output");
    return;
}

var pluginsDir = args[0];

if (!Directory.Exists(pluginsDir))
{
    Console.WriteLine($"Папка не найдена: {pluginsDir}");
    return;
}

Console.WriteLine($"Загрузка плагинов из: {Path.GetFullPath(pluginsDir)}\n");

var manager = new PluginManager();
manager.LoadPluginsFromDirectory(pluginsDir);

Console.WriteLine($"Загружено плагинов: {manager.LoadedPlugins.Count}\n");

Console.WriteLine("Порядок выполнения:");
foreach (var plugin in manager.LoadedPlugins)
{
    Console.WriteLine($"  - {plugin.Name}");
}

Console.WriteLine("\nВыполнение плагинов:");
manager.ExecuteAll();
