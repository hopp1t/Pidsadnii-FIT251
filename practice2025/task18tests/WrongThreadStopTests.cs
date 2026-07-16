using task18;
using Xunit;

namespace task18tests;

public class WrongThreadStopTests
{
    private class TestCommand : ICommand
    {
        public bool Executed { get; private set; }
        public void Execute() => Executed = true;
    }

    private class MockExceptionHandler : IExceptionHandler
    {
        public List<(ICommand Command, Exception Exception)> Handled { get; } = new();
        public void Handle(ICommand command, Exception exception) => Handled.Add((command, exception));
    }

    [Fact]
    public void HardStop_InWrongThread_ShouldBeHandledByExceptionHandler_AndNotStopServer()
    {
        var exceptionHandler = new MockExceptionHandler();
        var serverThread = new ServerThread(exceptionHandler);

        var cmd1 = new TestCommand();
        var cmd2 = new TestCommand();
        var wrongHardStop = new HardStop(999);

        serverThread.Start(); 
        serverThread.Enqueue(cmd1);
        serverThread.Enqueue(wrongHardStop);
        serverThread.Enqueue(cmd2);
        serverThread.EnqueueSoftStop();

        serverThread.Join();

        Assert.True(cmd1.Executed);
        Assert.True(cmd2.Executed);
        Assert.Single(exceptionHandler.Handled);
        Assert.IsType<InvalidOperationException>(exceptionHandler.Handled[0].Exception);
    }

    [Fact]
    public void SoftStop_InWrongThread_ShouldBeHandledByExceptionHandler_AndNotStopServer()
    {
        var exceptionHandler = new MockExceptionHandler();
        var serverThread = new ServerThread(exceptionHandler);

        var cmd1 = new TestCommand();
        var cmd2 = new TestCommand();
        var wrongSoftStop = new SoftStop(999);

        serverThread.Start(); 
        serverThread.Enqueue(cmd1);
        serverThread.Enqueue(wrongSoftStop);
        serverThread.Enqueue(cmd2);
        serverThread.EnqueueSoftStop();

        serverThread.Join();

        Assert.True(cmd1.Executed);
        Assert.True(cmd2.Executed);
        Assert.Single(exceptionHandler.Handled);
    }
}
