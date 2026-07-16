using task18;
using Xunit;
using System.Threading;
using System.Diagnostics;

namespace task18tests;

public class IdleCpuTests
{
    [Fact]
    public void IdleServerThread_ShouldNotConsumeSignificantCpuTime()
    {
        var serverThread = new ServerThread();
        serverThread.Start();

        Thread.Sleep(200); 

        serverThread.EnqueueSoftStop();
        
        var stopwatch = Stopwatch.StartNew();
        serverThread.Join();
        stopwatch.Stop();

        Assert.True(stopwatch.ElapsedMilliseconds < 100, 
            $"Поток должен завершаться мгновенно, но заняло {stopwatch.ElapsedMilliseconds} мс");
    }

    [Fact]
    public void ServerThread_ShouldHandleExceptionFromCommand_AndContinueWorking()
    {
        var handler = new MockExceptionHandler();
        var serverThread = new ServerThread(handler);

        var throwingCmd = new ThrowingCommand();
        var normalCmd = new NormalCommand();

        serverThread.Start();
        serverThread.Enqueue(throwingCmd);
        serverThread.Enqueue(normalCmd);
        serverThread.EnqueueSoftStop();
        serverThread.Join();

        Assert.True(normalCmd.Executed); 
        Assert.Single(handler.Handled);  
    }

    private class ThrowingCommand : ICommand
    {
        public void Execute() => throw new InvalidOperationException("Test exception");
    }

    private class NormalCommand : ICommand
    {
        public bool Executed { get; private set; }
        public void Execute() => Executed = true;
    }

    private class MockExceptionHandler : IExceptionHandler
    {
        public List<(ICommand Command, Exception Exception)> Handled { get; } = new();
        public void Handle(ICommand command, Exception exception) => Handled.Add((command, exception));
    }
}
