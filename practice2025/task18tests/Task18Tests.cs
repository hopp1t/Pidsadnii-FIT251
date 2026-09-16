using task18;
using Xunit;
using System.Threading;
using System.Diagnostics;

namespace task18tests;

public class Task18Tests
{
    private class TestCommand : ICommand { public bool Executed { get; private set; } public void Execute() => Executed = true; }
    private class ActionCommand : ICommand { private readonly Action _a; public ActionCommand(Action a) => _a = a; public void Execute() => _a(); }
    private class ThrowingCommand : ICommand { public void Execute() => throw new InvalidOperationException("Test"); }
    private class NormalCommand : ICommand { public bool Executed { get; private set; } public void Execute() => Executed = true; }
    
    private class CountingLongCommand : LongRunningCommand
    {
        private readonly int _total; public int Steps { get; private set; }
        public CountingLongCommand(int total) => _total = total;
        protected override void ExecuteStep() { Steps++; Thread.Sleep(1); }
        protected override bool IsWorkCompleted() => Steps >= _total;
    }

    private class MockExceptionHandler : IExceptionHandler
    {
        public List<(ICommand Command, Exception Exception)> Handled { get; } = new();
        public void Handle(ICommand c, Exception e) => Handled.Add((c, e));
    }

    [Fact] public void Scheduler_Empty_ShouldHaveNoCommands() => Assert.False(new RoundRobinScheduler().HasCommand());

    [Fact]
    public void Scheduler_Select_RemovesCommand()
    {
        var s = new RoundRobinScheduler();
        var cmd = new TestCommand();
        s.Add(cmd);
        Assert.Same(cmd, s.Select());
        Assert.False(s.HasCommand());
    }

    [Fact]
    public void LongRunningCommand_ExecutesInSteps()
    {
        var cmd = new CountingLongCommand(3);
        cmd.Execute(); Assert.False(cmd.IsCompleted); Assert.Equal(1, cmd.Steps);
        cmd.Execute(); Assert.False(cmd.IsCompleted); Assert.Equal(2, cmd.Steps);
        cmd.Execute(); Assert.True(cmd.IsCompleted); Assert.Equal(3, cmd.Steps);
        cmd.Execute(); Assert.Equal(3, cmd.Steps);
    }

    [Fact]
    public void ServerThread_ExecutesInstantCommands()
    {
        var server = new ServerThread();
        var cmd = new TestCommand();
        server.Start(); server.Enqueue(cmd); server.EnqueueSoftStop(); server.Join();
        Assert.True(cmd.Executed);
    }

    [Fact]
    public void ServerThread_ProcessesLongRunningInRoundRobin()
    {
        var server = new ServerThread();
        var long1 = new CountingLongCommand(3);
        var long2 = new CountingLongCommand(2);
        var instant = new TestCommand();

        server.Start();
        server.Enqueue(long1); server.Enqueue(long2); server.Enqueue(instant);
        server.EnqueueSoftStop(); server.Join();

        Assert.True(instant.Executed);
        Assert.True(long1.IsCompleted); Assert.Equal(3, long1.Steps);
        Assert.True(long2.IsCompleted); Assert.Equal(2, long2.Steps);
    }

    [Fact]
    public void ServerThread_HandlesExceptionsAndContinues()
    {
        var handler = new MockExceptionHandler();
        var server = new ServerThread(exceptionHandler: handler);
        var throwingCmd = new ThrowingCommand();
        var normalCmd = new NormalCommand();
        
        server.Start();
        server.Enqueue(throwingCmd);
        server.Enqueue(normalCmd);
        server.EnqueueSoftStop();
        server.Join();
    
        Assert.Single(handler.Handled);
        Assert.Same(throwingCmd, handler.Handled[0].Command);
        
        Assert.True(normalCmd.Executed);
    }

    [Fact]
    public void ServerThread_NoDeadlockOnSoftStopWithLongCommands()
    {
        var server = new ServerThread();
        var longCmd = new CountingLongCommand(5);
        server.Start();
        server.Enqueue(longCmd);
        Thread.Sleep(50);
        server.EnqueueSoftStop();
        
        bool finished = server.Join(2000);
        Assert.True(finished);
        Assert.True(longCmd.IsCompleted);
    }

    [Fact]
    public void ServerThread_HandlesHighLoad()
    {
        var server = new ServerThread();
        int count = 0;
        var lockObj = new object();
        server.Start();
        for (int i = 0; i < 1000; i++) server.Enqueue(new ActionCommand(() => { lock(lockObj) count++; }));
        server.EnqueueSoftStop();
        server.Join();
        Assert.Equal(1000, count);
    }

    [Fact]
    public void ServerThread_IdleNoCpuSpike()
    {
        var server = new ServerThread();
        server.Start();
        Thread.Sleep(200); 
        server.EnqueueSoftStop();
        var sw = Stopwatch.StartNew();
        server.Join();
        sw.Stop();
        Assert.True(sw.ElapsedMilliseconds < 100, $"Завершение заняло {sw.ElapsedMilliseconds} мс");
    }
}
