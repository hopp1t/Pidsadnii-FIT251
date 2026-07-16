using task17;
using Xunit;

namespace task17tests;

public class LongRunningCommandTests
{
    [Fact]
    public void LongRunningCommand_ShouldExecuteInSteps()
    {
        var command = new TestLongRunningCommand(totalSteps: 5);
        Assert.False(command.IsCompleted);

        command.Execute(); 
        Assert.False(command.IsCompleted);
        Assert.Equal(1, command.ExecutionCount);

        command.Execute(); 
        Assert.False(command.IsCompleted);
        Assert.Equal(2, command.ExecutionCount);

        command.Execute(); 
        command.Execute(); 
        command.Execute(); 
        Assert.True(command.IsCompleted);
        Assert.Equal(5, command.ExecutionCount);
    }

    [Fact]
    public void CompletedCommand_ShouldNotExecuteFurther()
    {
        var command = new TestLongRunningCommand(totalSteps: 1);
        command.Execute();
        Assert.True(command.IsCompleted);

        command.Execute();
        command.Execute();
        Assert.Equal(1, command.ExecutionCount);
    }

    private class TestLongRunningCommand : LongRunningCommand
    {
        private readonly int _totalSteps;
        public int ExecutionCount { get; private set; }

        public TestLongRunningCommand(int totalSteps)
        {
            _totalSteps = totalSteps;
        }

        protected override void ExecuteStep()
        {
            ExecutionCount++;
        }

        protected override bool IsWorkCompleted()
        {
            return ExecutionCount >= _totalSteps;
        }
    }
}
