using task18;
using Xunit;

namespace task18tests;

public class ServerThreadV2Tests
{
    private class InstantCommand : ICommand
    {
        public bool Executed { get; private set; }
        public void Execute() => Executed = true;
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
        }

        protected override bool IsWorkCompleted()
        {
            return StepCount >= _totalSteps;
        }
    }

    [Fact]
    public void ServerThreadV2_ShouldExecuteInstantCommandsImmediately()
    {
        var server = new ServerThreadV2();
        var cmd = new InstantCommand();

        server.Start();
        server.Enqueue(cmd);
        server.EnqueueSoftStop();
        server.Join();

        Assert.True(cmd.Executed);
    }

    [Fact]
    public void ServerThreadV2_ShouldProcessLongRunningCommandsInRoundRobin()
    {
        var server = new ServerThreadV2();
        var longCmd1 = new CountingLongCommand(3);
        var longCmd2 = new CountingLongCommand(2);
        var instantCmd = new InstantCommand();

        server.Start();
        server.Enqueue(longCmd1);
        server.Enqueue(longCmd2);
        server.Enqueue(instantCmd); 
        server.EnqueueSoftStop();
        server.Join();

        Assert.True(instantCmd.Executed);
        Assert.True(longCmd1.IsCompleted);
        Assert.True(longCmd2.IsCompleted);
        Assert.Equal(3, longCmd1.StepCount);
        Assert.Equal(2, longCmd2.StepCount);
    }

    [Fact]
    public void ServerThreadV2_ShouldNotBlockOnEmptyQueueWhenSchedulerHasWork()
    {
        var server = new ServerThreadV2();
        var longCmd = new CountingLongCommand(100);

        server.Start();
        server.Enqueue(longCmd);

        Thread.Sleep(100);

        server.EnqueueSoftStop();
        server.Join();

        Assert.True(longCmd.IsCompleted);
    }
}
