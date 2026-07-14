using task17;
using Xunit;

namespace task17tests;

public class ICommandTests
{
    private class SimpleCommand : ICommand
    {
        public bool WasExecuted { get; private set; }
        public void Execute() => WasExecuted = true;
    }

    [Fact]
    public void Command_ShouldBeExecutable()
    {
        var command = new SimpleCommand();
        Assert.False(command.WasExecuted);
        command.Execute();
        Assert.True(command.WasExecuted);
    }

    [Fact]
    public void Command_CanThrowException()
    {
        var command = new ThrowingCommand();
        Assert.Throws<InvalidOperationException>(() => command.Execute());
    }

    private class ThrowingCommand : ICommand
    {
        public void Execute() => throw new InvalidOperationException("Test");
    }
}
