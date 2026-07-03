using PluginLoader;
using Xunit;

namespace task10tests;

public class PluginManagerTests
{
    private string GetPluginsDirectory()
    {
        var testDir = AppDomain.CurrentDomain.BaseDirectory;
        var pluginsDir = Path.Combine(testDir, "..", "..", "..", "..", "SamplePlugins", "bin", "Debug", "net9.0");
        return Path.GetFullPath(pluginsDir);
    }

    [Fact]
    public void LoadPluginsFromDirectory_ShouldLoadOnlyPluginsWithAttribute()
    {
        var manager = new PluginManager();
        var pluginsDir = GetPluginsDirectory();

        manager.LoadPluginsFromDirectory(pluginsDir);

        Assert.Contains(manager.LoadedPlugins, p => p.Name == "PluginA");
        Assert.Contains(manager.LoadedPlugins, p => p.Name == "PluginB");
    }

    [Fact]
    public void LoadPluginsFromDirectory_ShouldRespectDependencies()
    {
        var manager = new PluginManager();
        var pluginsDir = GetPluginsDirectory();

        manager.LoadPluginsFromDirectory(pluginsDir);

        var indexA = manager.LoadedPlugins.ToList().FindIndex(p => p.Name == "PluginA");
        var indexB = manager.LoadedPlugins.ToList().FindIndex(p => p.Name == "PluginB");

        Assert.True(indexA >= 0 && indexB >= 0, "Оба плагина должны быть загружены");
        Assert.True(indexA < indexB, 
            $"PluginA (index {indexA}) должен быть загружен раньше PluginB (index {indexB})");
    }
}
