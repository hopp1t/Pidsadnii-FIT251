using PluginLib;

namespace B_Plugin;

[PluginLoad]
[PluginDependency("PluginA")]
public class PluginB : IPlugin
{
    public string Name => "PluginB";
    public void Execute() => Console.WriteLine("B");
}
