using PluginLib;

namespace task10tests;

[PluginLoad]
[PluginDependency("PluginA")]
public class DependencyPluginB : IPlugin
{
    public string Name => "PluginB";
    public void Execute() { }
}
