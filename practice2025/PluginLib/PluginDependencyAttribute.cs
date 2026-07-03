namespace PluginLib;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class PluginDependencyAttribute : Attribute
{
    public string RequiredPluginName { get; }

    public PluginDependencyAttribute(string requiredPluginName)
    {
        RequiredPluginName = requiredPluginName;
    }
}
