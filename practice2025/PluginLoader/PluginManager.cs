using PluginLib;

namespace PluginLoader;

public class PluginManager
{
    private readonly List<IPlugin> _loadedPlugins = new();

    public IReadOnlyList<IPlugin> LoadedPlugins => _loadedPlugins.AsReadOnly();

    public void LoadPluginsFromDirectory(string directoryPath)
    {
        throw new NotImplementedException();
    }

    public void ExecuteAll()
    {
        foreach (var plugin in _loadedPlugins)
        {
            plugin.Execute();
        }
    }
}
