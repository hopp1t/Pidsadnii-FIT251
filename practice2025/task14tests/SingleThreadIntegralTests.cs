using task13;
using Xunit;

namespace task13tests;

public class SingleThreadIntegralTests
{
    private static readonly Func<double, double> SIN = x => Math.Sin(x);
    private static readonly Func<double, double> X = x => x;
    private static readonly Func<double, double> X2 = x => x * x;
    private static readonly Func<double, double> CONST = x => 1.0;

    [Fact]
    public void Solve_SinFromMinus100To100_ShouldReturnZero()
    {
        var result = SingleThreadIntegral.Solve(-100, 100, SIN, 1e-4);
        Assert.Equal(0, result, 4);
    }

    [Fact]
    public void Solve_XFrom0To1_ShouldReturnHalf()
    {
        var result = SingleThreadIntegral.Solve(0, 1, X, 1e-5);
        Assert.Equal(0.5, result, 5);
    }

    [Fact]
    public void Solve_X2From0To1_ShouldReturnOneThird()
    {
        var result = SingleThreadIntegral.Solve(0, 1, X2, 1e-6);
        Assert.Equal(1.0 / 3.0, result, 5);
    }

    [Fact]
    public void Solve_ConstFrom0To10_ShouldReturn10()
    {
        var result = SingleThreadIntegral.Solve(0, 10, CONST, 1e-4);
        Assert.Equal(10.0, result, 4);
    }

    [Fact]
    public void Solve_NullFunction_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(
            () => SingleThreadIntegral.Solve(0, 1, null!, 1e-5));
    }

    [Fact]
    public void Solve_NegativeStep_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => SingleThreadIntegral.Solve(0, 1, X, -1e-5));
    }
}
