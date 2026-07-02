using System.Reflection;
using FileSystemCommands;
using Xunit;
namespace task09tests;

public class MetadataTests
{
    [Fact]
    public void DirectorySizeCommand_Constructor_ShouldHaveOneStringParameter()
    {
        var type = typeof(DirectorySizeCommand);
        var ctor = type.GetConstructors().First();
        var parameters = ctor.GetParameters();

        Assert.Single(parameters);
        Assert.Equal("directoryPath", parameters[0].Name);
        Assert.Equal(typeof(string), parameters[0].ParameterType);
    }

    [Fact]
    public void FindFilesCommand_Constructor_ShouldHaveTwoStringParameters()
    {
        var type = typeof(FindFilesCommand);
        var ctor = type.GetConstructors().First();
        var parameters = ctor.GetParameters();

        Assert.Equal(2, parameters.Length);
        Assert.Equal("directoryPath", parameters[0].Name);
        Assert.Equal("pattern", parameters[1].Name);
    }

    [Fact]
    public void Commands_ShouldHaveExecuteMethodWithNoParameters()
    {
        var methods1 = typeof(DirectorySizeCommand).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        var execute1 = methods1.FirstOrDefault(m => m.Name == "Execute");
        
        Assert.NotNull(execute1);
        Assert.Empty(execute1.GetParameters());
    }
}
