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
        Assert.Equal(0, calculator.Add(-5, 5));
    }

    [Fact]
    public void Minus_ShouldReturnDifference()
    {
        var calculator = CalculatorGenerator.CreateCalculator();
        Assert.Equal(1, calculator.Minus(5, 4));
        Assert.Equal(-10, calculator.Minus(-5, 5));
    }

    [Fact]
    public void Mul_ShouldReturnProduct()
    {
        var calculator = CalculatorGenerator.CreateCalculator();
        Assert.Equal(12, calculator.Mul(3, 4));
        Assert.Equal(-15, calculator.Mul(-3, 5));
    }

    [Fact]
    public void Div_ShouldReturnQuotient()
    {
        var calculator = CalculatorGenerator.CreateCalculator();
        Assert.Equal(5, calculator.Div(10, 2));
        Assert.Equal(3, calculator.Div(10, 3));
    }

    [Fact]
    public void CreateCalculator_WithInvalidCode_ShouldThrowException()
    {
        var invalidCode = @"
            public class Calculator
            {
                public int Add(int a, int b) => a + 
            }";
        
        Assert.ThrowsAny<Exception>(() => 
            CalculatorGenerator.CreateCalculatorFromCode(invalidCode));
    }

    [Fact]
    public void Calculator_ShouldBeCompiledFromString()
    {
        var calculator = CalculatorGenerator.CreateCalculator();
        var type = calculator.GetType();

        Assert.Contains("CalculatorAssembly", type.Assembly.FullName);
        Assert.True(typeof(ICalculator).IsAssignableFrom(type));
    }

    [Fact]
    public void CreateCalculator_ShouldUseInterfaceForMethodCalls()
    {
        var calculator = CalculatorGenerator.CreateCalculator();
        
        ICalculator calc = calculator;
        Assert.Equal(5, calc.Add(2, 3));
        Assert.Equal(1, calc.Minus(5, 4));
        Assert.Equal(6, calc.Mul(2, 3));
        Assert.Equal(5, calc.Div(10, 2));
    }
}
