using PluginLoader;
using Xunit;
using System.Reflection;

namespace task10tests;

public class PluginManagerTests
{
    [Fact]
    public void LoadPluginsFromDirectory_ShouldLoadOnlyPluginsWithAttribute()
    {
        // Arrange
        var manager = new PluginManager();
        var assemblyPath = Assembly.GetExecutingAssembly().Location;
        var directoryPath = Path.GetDirectoryName(assemblyPath)!;

        // Act
        manager.LoadPluginsFromDirectory(directoryPath);

        // Assert
        Assert.Contains(manager.LoadedPlugins, p => p.Name == "RealPlugin");
        Assert.DoesNotContain(manager.LoadedPlugins, p => p.Name == "FakePlugin");
    }
}
