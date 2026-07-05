using task11;
using Xunit;

namespace task11tests;

public class CalculatorTests
{
    [Fact]
    public void CreateCalculator_ShouldReturnInstance()
    {
        var calculator = CalculatorGenerator.CreateCalculator();
        Assert.NotNull(calculator);
        Assert.IsAssignableFrom<ICalculator>(calculator);
    }

    [Fact]
    public void Add_ShouldReturnSum()
    {
        var calculator = CalculatorGenerator.CreateCalculator();
        Assert.Equal(7, calculator.Add(3, 4));
    }

    [Fact]
    public void Minus_ShouldReturnDifference()
    {
        var calculator = CalculatorGenerator.CreateCalculator();
        Assert.Equal(1, calculator.Minus(5, 4));
    }

    [Fact]
    public void Mul_ShouldReturnProduct()
    {
        var calculator = CalculatorGenerator.CreateCalculator();
        Assert.Equal(12, calculator.Mul(3, 4));
    }

    [Fact]
    public void Div_ShouldReturnQuotient()
    {
        var calculator = CalculatorGenerator.CreateCalculator();
        Assert.Equal(5, calculator.Div(10, 2));
    }
    [Fact]
    public void Calculator_ShouldBeDynamicallyGenerated()
    {
        var calculator = CalculatorGenerator.CreateCalculator();
        var type = calculator.GetType();
        
        Assert.Equal("DynamicCalculator", type.Name);
        Assert.True(type.Assembly.IsDynamic);
        Assert.True(typeof(ICalculator).IsAssignableFrom(type));
    }
}
