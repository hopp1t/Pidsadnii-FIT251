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
    public void ServerThreadV2_ShouldCompleteWithFinishingLongCommands()
    {
        var server = new ServerThreadV2();
        var longCmd = new CountingLongCommand(5);

        server.Start();
        server.Enqueue(longCmd);
        
        Thread.Sleep(50);
        
        server.EnqueueSoftStop();
        
        bool finished = server.Join(2000);
        
        Assert.True(finished);
        Assert.True(longCmd.IsCompleted);
    }

    private class ActionCommand : ICommand
    {
        private readonly Action _action;
        public ActionCommand(Action action) => _action = action;
        public void Execute() => _action();
    }

    private class CountingLongCommand : LongRunningCommand
    {
        private readonly int _totalSteps;
        public int StepCount { get; private set; }

        public CountingLongCommand(int totalSteps)
        {
            _totalSteps = totalSteps;
        }

        protected override void ExecuteStep()
        {
            StepCount++;
            Thread.Sleep(1);
        }

        protected override bool IsWorkCompleted() => StepCount >= _totalSteps;
    }
}
