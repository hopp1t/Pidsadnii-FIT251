using task17;
using Xunit;

namespace task17tests;

public class SoftStopTests
{
    [Fact]
    public void SoftStop_ExecutedInTargetThread_ShouldNotThrow()
    {
        var command = new SoftStop(Thread.CurrentThread.ManagedThreadId);
        var exception = Record.Exception(() => command.Execute());
        Assert.Null(exception);
    }

    [Fact]
    public void SoftStop_ExecutedInWrongThread_ShouldThrow()
    {
        var command = new SoftStop(999);
        Assert.Throws<InvalidOperationException>(() => command.Execute());
    }
}
