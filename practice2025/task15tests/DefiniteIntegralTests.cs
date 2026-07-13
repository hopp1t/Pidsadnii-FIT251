using task15;
using Xunit;

namespace task15tests;

public class DefiniteIntegralTests
{
    private static readonly Func<double, double> X = x => x;
    private static readonly Func<double, double> SIN = x => Math.Sin(x);

    [Fact]
    public void IntegralOfX_FromMinus1To1_With2Threads_ShouldReturnZero()
    {
        var result = DefiniteIntegral.Solve(-1, 1, X, 1e-4, 2);
        Assert.Equal(0, result, 4);
    }

    [Fact]
    public void IntegralOfSin_FromMinus1To1_With8Threads_ShouldReturnZero()
    {
        var result = DefiniteIntegral.Solve(-1, 1, SIN, 1e-5, 8);
        Assert.Equal(0, result, 4);
    }

    [Fact]
    public void IntegralOfX_From0To5_With8Threads_ShouldReturn12_5()
    {
        var result = DefiniteIntegral.Solve(0, 5, X, 1e-6, 8);
        Assert.Equal(12.5, result, 5);
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
}
