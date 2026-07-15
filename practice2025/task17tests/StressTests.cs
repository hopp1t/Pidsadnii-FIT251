using task17;
using Xunit;
using System.Threading;

namespace task17tests;

public class StressTests
{
    [Fact]
    public void ServerThreadV2_ShouldHandleHighLoad()
    {
        var server = new ServerThreadV2();
        const int commandCount = 1000;
        var executedCount = 0;
        var lockObj = new object();

        server.Start();

        for (int i = 0; i < commandCount; i++)
        {
            server.Enqueue(new ActionCommand(() =>
            {
                lock (lockObj) { executedCount++; }
            }));
        }

        server.EnqueueSoftStop();
        server.Join();

        Assert.Equal(commandCount, executedCount);
    }

    [Fact]
    public void ServerThreadV2_ShouldNotDeadlockWithLongRunningCommands()
    {
        var server = new ServerThreadV2();
        var longCmd = new NeverEndingCommand();

        server.Start();
        server.Enqueue(longCmd);

        Thread.Sleep(100);

        server.EnqueueSoftStop();

        bool finished = server.Join(3000);

        Assert.True(finished, "Server should complete within 3 seconds without deadlock");
    }

    private class ActionCommand : ICommand
    {
        private readonly Action _action;
        public ActionCommand(Action action) => _action = action;
        public void Execute() => _action();
    }

    private class NeverEndingCommand : LongRunningCommand
    {
        protected override void ExecuteStep() 
        { 
            Thread.Sleep(1); 
        }
        protected override bool IsWorkCompleted() => false; 
    }
}
