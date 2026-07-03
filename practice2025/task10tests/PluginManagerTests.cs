using PluginLoader;
using Xunit;

namespace task10tests;

public class PluginManagerTests
{
    private string GetPluginsDirectory()
    {
        var testDir = AppDomain.CurrentDomain.BaseDirectory;
        var pluginsDir = Path.Combine(testDir, "..", "..", "..", "..", "plugins_output");
        return Path.GetFullPath(pluginsDir);
    }

    [Fact]
    public void LoadPluginsFromDirectory_ShouldLoadOnlyPluginsWithAttribute()
    {
        var manager = new PluginManager();
        var pluginsDir = GetPluginsDirectory();

        manager.LoadPluginsFromDirectory(pluginsDir);

        Assert.Equal(3, manager.LoadedPlugins.Count);
        Assert.Contains(manager.LoadedPlugins, p => p.Name == "PluginA");
        Assert.Contains(manager.LoadedPlugins, p => p.Name == "PluginB");
        Assert.Contains(manager.LoadedPlugins, p => p.Name == "PluginC");
    }

    [Fact]
    public void LoadPluginsFromDirectory_ShouldRespectDependencies()
    {
        var manager = new PluginManager();
        var pluginsDir = GetPluginsDirectory();

        manager.LoadPluginsFromDirectory(pluginsDir);

        var names = manager.LoadedPlugins.Select(p => p.Name).ToList();
        
        Assert.Equal(new[] { "PluginA", "PluginB", "PluginC" }, names);
    }
}
