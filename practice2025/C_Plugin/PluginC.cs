using PluginLib;

namespace C_Plugin;

[PluginLoad]
[PluginDependency("PluginB")]
public class PluginC : IPlugin
{
    public string Name => "PluginC";
    public void Execute() => Console.WriteLine("C");
}
