using System.Reflection;
using CommandLib;
using FileSystemCommands;
using Xunit;

namespace task09tests;

public class AttributeTests
{
    [Fact]
    public void DirectorySizeCommand_ShouldHaveDisplayNameAttribute()
    {
        var type = typeof(DirectorySizeCommand);
        var attr = type.GetCustomAttribute<DisplayNameAttribute>();
        
        Assert.NotNull(attr);
        Assert.Equal("Вычислить размер каталога", attr.DisplayName);
    }

    [Fact]
    public void DirectorySizeCommand_ShouldHaveVersionAttribute()
    {
        var type = typeof(DirectorySizeCommand);
        var attr = type.GetCustomAttribute<VersionAttribute>();
        
        Assert.NotNull(attr);
        Assert.Equal(1, attr.Major);
        Assert.Equal(0, attr.Minor);
    }

    [Fact]
    public void FindFilesCommand_ShouldHaveAttributes()
    {
        var type = typeof(FindFilesCommand);
        
        Assert.NotNull(type.GetCustomAttribute<DisplayNameAttribute>());
        Assert.NotNull(type.GetCustomAttribute<VersionAttribute>());
    }
}
