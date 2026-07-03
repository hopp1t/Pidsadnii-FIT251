using PluginLib;

namespace task10tests;

[PluginLoad]
public class RealPlugin : IPlugin
{
    public string Name => "RealPlugin";
    public void Execute() { }
}
