using task14;
using Xunit;

namespace task14tests;

public class DefiniteIntegralTests
{
    private static readonly Func<double, double> X = x => x;
    private static readonly Func<double, double> SIN = x => Math.Sin(x);
    private static readonly Func<double, double> X2 = x => x * x;
    private static readonly Func<double, double> CONST = x => 1.0;

    [Fact]
    public void IntegralOfX_ShouldReturnZero()
    {
        var result = DefiniteIntegral.Solve(-1, 1, X, 1e-4, 2);
        Assert.Equal(0, result, 4);
    }

    [Fact]
    public void IntegralOfSin_ShouldReturnZero()
    {
        var result = DefiniteIntegral.Solve(-1, 1, SIN, 1e-5, 8);
        Assert.Equal(0, result, 4);
    }

    [Fact]
    public void IntegralOfX_ShouldReturn10()
    {
        var result = DefiniteIntegral.Solve(0, 5, X, 1e-6, 8);
        Assert.Equal(10, result, 5);
    }

    [Fact]
    public void ZeroThreads_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => DefiniteIntegral.Solve(0, 1, X, 1e-5, 0));
    }

    [Fact]
    public void NullFunction_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(
            () => DefiniteIntegral.Solve(0, 1, null!, 1e-5, 2));
    }

    [Fact]
    public void NegativeStep_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => DefiniteIntegral.Solve(0, 1, X, -1e-5, 2));
    }

    [Fact]
    public void IntegralOfX2_From0To1_ShouldReturnOneThird()
    {
        var result = DefiniteIntegral.Solve(0, 1, X2, 1e-6, 4);
        Assert.Equal(1.0 / 3.0, result, 5);
    }

    [Fact]
    public void IntegralOfConst_From0To10_ShouldReturn10()
    {
        var result = DefiniteIntegral.Solve(0, 10, CONST, 1e-4, 4);
        Assert.Equal(10.0, result, 4);
    }

    [Fact]
    public void WithSingleThread_ShouldWork()
    {
        var result = DefiniteIntegral.Solve(0, 1, X, 1e-5, 1);
        Assert.Equal(0.5, result, 5);
    }

    [Fact]
    public void WithManyThreads_ShouldWork()
    {
        var result = DefiniteIntegral.Solve(0, 1, X, 1e-5, 16);
        Assert.Equal(0.5, result, 5);
    }
}
