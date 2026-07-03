using PluginLib;

namespace task10tests;

[PluginLoad]
public class DependencyPluginA : IPlugin
{
    public string Name => "PluginA";
    public void Execute() { }
}
