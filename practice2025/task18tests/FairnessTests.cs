using task18;
using Xunit;

namespace task18tests;

public class FairnessTests
{
    private class TrackingLongCommand : LongRunningCommand
    {
        private readonly int _totalSteps;
        public List<DateTime> ExecutionTimestamps { get; } = new();

        public TrackingLongCommand(int totalSteps)
        {
            _totalSteps = totalSteps;
        }

        protected override void ExecuteStep()
        {
            ExecutionTimestamps.Add(DateTime.UtcNow);
            Thread.Sleep(1); // Small delay to simulate work
        }

        protected override bool IsWorkCompleted()
        {
            return ExecutionTimestamps.Count >= _totalSteps;
        }
    }

    [Fact]
    public void RoundRobinScheduler_ShouldProvideFairExecution()
    {
        var server = new ServerThreadV2();
        var cmd1 = new TrackingLongCommand(5);
        var cmd2 = new TrackingLongCommand(5);
        var cmd3 = new TrackingLongCommand(5);

        server.Start();
        server.Enqueue(cmd1);
        server.Enqueue(cmd2);
        server.Enqueue(cmd3);
        server.EnqueueSoftStop();
        server.Join();

        Assert.Equal(5, cmd1.ExecutionTimestamps.Count);
        Assert.Equal(5, cmd2.ExecutionTimestamps.Count);
        Assert.Equal(5, cmd3.ExecutionTimestamps.Count);

        Assert.True(cmd1.ExecutionTimestamps[0] < cmd1.ExecutionTimestamps[4]);
        Assert.True(cmd2.ExecutionTimestamps[0] < cmd2.ExecutionTimestamps[4]);
    }

    [Fact]
    public void ServerThreadV2_ShouldHandleMixedCommandTypes()
    {
        var server = new ServerThreadV2();
        var instantCommands = new List<InstantCommand>();
        var longCommands = new List<TrackingLongCommand>();

        server.Start();

        for (int i = 0; i < 3; i++)
        {
            var instant = new InstantCommand();
            instantCommands.Add(instant);
            server.Enqueue(instant);

            var longCmd = new TrackingLongCommand(2);
            longCommands.Add(longCmd);
            server.Enqueue(longCmd);
        }

        server.EnqueueSoftStop();
        server.Join();

        Assert.All(instantCommands, cmd => Assert.True(cmd.Executed));
        Assert.All(longCommands, cmd => Assert.True(cmd.IsCompleted));
    }

    private class InstantCommand : ICommand
    {
        public bool Executed { get; private set; }
        public void Execute() => Executed = true;
    }
}
