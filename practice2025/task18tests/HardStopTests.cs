using task18;
using Xunit;

namespace task18tests;

public class HardStopTests
{
    [Fact]
    public void HardStop_ExecutedInTargetThread_ShouldNotThrow()
    {
        var command = new HardStop(Thread.CurrentThread.ManagedThreadId);
        var exception = Record.Exception(() => command.Execute());
        Assert.Null(exception);
    }

    [Fact]
    public void HardStop_ExecutedInWrongThread_ShouldThrow()
    {
        var command = new HardStop(999);
        Assert.Throws<InvalidOperationException>(() => command.Execute());
    }
}
