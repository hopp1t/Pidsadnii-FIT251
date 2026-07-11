using task13;
using Xunit;

namespace task13tests;

public class IntegralComparisonTests
{
    private static readonly Func<double, double> SIN = x => Math.Sin(x);

    [Fact]
    public void SingleThread_AndMultiThread_ShouldReturnSameResult_ForSin()
    {
        var singleResult = SingleThreadIntegral.Solve(-100, 100, SIN, 1e-4);
        var multiResult = DefiniteIntegral.Solve(-100, 100, SIN, 1e-4, 4);
        Assert.Equal(singleResult, multiResult, 10);
    }

    [Fact]
    public void SingleThread_AndMultiThread_ShouldReturnSameResult_WithDifferentThreads()
    {
        var singleResult = SingleThreadIntegral.Solve(-100, 100, SIN, 1e-4);
        
        foreach (var threads in new[] { 1, 2, 4, 8, 16 })
        {
            var multiResult = DefiniteIntegral.Solve(-100, 100, SIN, 1e-4, threads);
            Assert.Equal(singleResult, multiResult, 10);
        }
    }
}
