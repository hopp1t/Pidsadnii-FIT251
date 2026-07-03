using PluginLib;

namespace task10tests;

// Обычный класс, без атрибута [PluginLoad]
public class FakePlugin : IPlugin
{
    public string Name => "FakePlugin";
    public void Execute() { }
}
