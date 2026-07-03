using PluginLib;
using System.Reflection;

namespace PluginLoader;

public class PluginManager
{
    private readonly List<IPlugin> _loadedPlugins = new();

    public IReadOnlyList<IPlugin> LoadedPlugins => _loadedPlugins.AsReadOnly();

    public void LoadPluginsFromDirectory(string directoryPath)
    {
        _loadedPlugins.Clear();
        if (!Directory.Exists(directoryPath)) return;

        var dllFiles = Directory.GetFiles(directoryPath, "*.dll");

        foreach (var dll in dllFiles)
        {
            try
            {
                var assembly = Assembly.LoadFrom(dll);
                foreach (var type in assembly.GetTypes())
                {
                    if (typeof(IPlugin).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
                    {
                        var instance = Activator.CreateInstance(type) as IPlugin;
                        if (instance != null)
                        {
                            _loadedPlugins.Add(instance);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки {dll}: {ex.Message}");
            }
        }
    }

    public void ExecuteAll()
    {
        foreach (var plugin in _loadedPlugins)
        {
            plugin.Execute();
        }
    }
}
