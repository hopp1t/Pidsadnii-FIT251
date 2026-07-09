using task14;
using Xunit;

namespace task14tests;

public class DefiniteIntegralTests
{
    private static readonly Func<double, double> X = x => x;
    private static readonly Func<double, double> SIN = x => Math.Sin(x);

    [Fact]
    public void Solve_IntegralOfX_FromMinus1To1_With2Threads_ShouldReturnZero()
    {
        var result = DefiniteIntegral.Solve(-1, 1, X, 1e-4, 2);
        Assert.Equal(0, result, 4);
    }

    [Fact]
    public void Solve_IntegralOfSin_FromMinus1To1_With8Threads_ShouldReturnZero()
    {
        var result = DefiniteIntegral.Solve(-1, 1, SIN, 1e-5, 8);
        Assert.Equal(0, result, 4);
    }

    [Fact]
    public void Solve_IntegralOfX_From0To5_With8Threads_ShouldReturn10()
    {
        var result = DefiniteIntegral.Solve(0, 5, X, 1e-6, 8);
        Assert.Equal(10, result, 5);
    }
}
