using Xunit;
using task05;

namespace task05tests;

public class TestClass
{
    public int PublicField;
    private string _privateField = string.Empty;
    public int Property { get; set; }

    public void Method() { }

    public int Add(int a, int b) => a + b;
}

[Serializable]
public class AttributedClass { }

public class PlainClass { }

public class ClassAnalyzerTests
{
    [Fact]
    public void GetPublicMethods_ReturnsCorrectMethods()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var methods = analyzer.GetPublicMethods();

        Assert.Contains("Method", methods);
        Assert.Contains("Add", methods);
    }

    [Fact]
    public void GetPublicMethods_DoesNotReturnObjectMethods()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var methods = analyzer.GetPublicMethods().ToList();

        Assert.DoesNotContain("ToString", methods);
        Assert.DoesNotContain("Equals", methods);
    }

    [Fact]
    public void GetAllFields_IncludesPrivateFields()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var fields = analyzer.GetAllFields();

        Assert.Contains("_privateField", fields);
    }

    [Fact]
    public void GetAllFields_IncludesPublicFields()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var fields = analyzer.GetAllFields();

        Assert.Contains("PublicField", fields);
    }

    [Fact]
    public void GetProperties_ReturnsPropertyNames()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var properties = analyzer.GetProperties();

        Assert.Contains("Property", properties);
    }

    [Fact]
    public void HasAttribute_ReturnsTrueForAttributedClass()
    {
        var analyzer = new ClassAnalyzer(typeof(AttributedClass));
        Assert.True(analyzer.HasAttribute<SerializableAttribute>());
    }

    [Fact]
    public void HasAttribute_ReturnsFalseForPlainClass()
    {
        var analyzer = new ClassAnalyzer(typeof(PlainClass));
        Assert.False(analyzer.HasAttribute<SerializableAttribute>());
    }

    [Fact]
    public void GetMethodParams_ReturnsParametersAndReturnType()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var result = analyzer.GetMethodParams("Add").ToList();

        Assert.Contains("Int32 a", result);
        Assert.Contains("Int32 b", result);
        Assert.Contains("Returns: Int32", result);
    }

    [Fact]
    public void GetMethodParams_MethodWithoutParameters_ReturnsOnlyReturnType()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var result = analyzer.GetMethodParams("Method").ToList();

        Assert.Single(result);
        Assert.Contains("Returns: Void", result);
    }

    [Fact]
    public void GetMethodParams_InvalidMethodName_ThrowsArgumentException()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        Assert.Throws<ArgumentException>(() => analyzer.GetMethodParams("NonExistent").ToList());
    }

    [Fact]
    public void Constructor_NullType_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ClassAnalyzer(null!));
    }
}
