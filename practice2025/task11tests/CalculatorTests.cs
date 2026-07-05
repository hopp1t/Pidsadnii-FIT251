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
}
