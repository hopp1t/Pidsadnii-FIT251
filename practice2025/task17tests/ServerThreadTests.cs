using task17;
using Xunit;

namespace task17tests;

public class ServerThreadTests
{
    private class TestCommand : ICommand
    {
        public bool Executed { get; private set; }
        public void Execute() => Executed = true;
    }

    [Fact]
    public void ServerThread_ShouldExecuteEnqueuedCommand()
    {
        var serverThread = new ServerThread();
        var cmd = new TestCommand();

        serverThread.Enqueue(cmd);
        serverThread.EnqueueSoftStop();
        serverThread.Start();
        serverThread.Join();

        Assert.True(cmd.Executed);
    }

    [Fact]
    public void ServerThread_ShouldExecuteMultipleCommandsInOrder()
    {
        var serverThread = new ServerThread();
        var executionOrder = new List<int>();
        var lockObj = new object();

        for (int i = 0; i < 5; i++)
        {
            var index = i;
            serverThread.Enqueue(new ActionCommand(() =>
            {
                lock (lockObj) { executionOrder.Add(index); }
            }));
        }

        serverThread.EnqueueSoftStop();
        serverThread.Start();
        serverThread.Join();

        Assert.Equal(new[] { 0, 1, 2, 3, 4 }, executionOrder);
    }

    [Fact]
    public void HardStop_ShouldStopImmediately_LeavingCommandsUnexecuted()
    {
        var serverThread = new ServerThread();
        var cmd1 = new TestCommand();
        var cmd2 = new TestCommand();

        serverThread.Enqueue(cmd1);
        serverThread.EnqueueHardStop();
        serverThread.Enqueue(cmd2);

        serverThread.Start();
        serverThread.Join();

        Assert.True(cmd1.Executed);
        Assert.False(cmd2.Executed);
    }

    [Fact]
    public void SoftStop_ShouldExecuteAllCommandsBeforeStopping()
    {
        var serverThread = new ServerThread();
        var cmd1 = new TestCommand();
        var cmd2 = new TestCommand();

        serverThread.Enqueue(cmd1);
        serverThread.EnqueueSoftStop();
        serverThread.Enqueue(cmd2);

        serverThread.Start();
        serverThread.Join();

        Assert.True(cmd1.Executed);
        Assert.True(cmd2.Executed);
    }

    private class ActionCommand : ICommand
    {
        private readonly Action _action;
        public ActionCommand(Action action) => _action = action;
        public void Execute() => _action();
    }
}
