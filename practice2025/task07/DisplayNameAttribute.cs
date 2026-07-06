namespace task07;

using System;

[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property,
    Inherited = false,
    AllowMultiple = false)]
public class DisplayNameAttribute : Attribute
{
    public string DisplayName { get; }

    public DisplayNameAttribute(string displayName)
    {
        DisplayName = displayName;
    }
}
