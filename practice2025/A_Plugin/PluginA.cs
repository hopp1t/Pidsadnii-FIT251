using PluginLib;

namespace A_Plugin;

[PluginLoad]
public class PluginA : IPlugin
{
    public string Name => "PluginA";
    public void Execute() => Console.WriteLine("A");
}
