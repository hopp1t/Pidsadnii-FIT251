using task17;
using Xunit;
using System.Collections.Concurrent; 
using System.Threading;              

namespace task17tests;
public class SchedulerTests
{
    private class TestCommand : ICommand
    {
        public bool Executed { get; private set; }
        public void Execute() => Executed = true;
    }

    [Fact]
    public void EmptyScheduler_ShouldHaveNoCommands()
    {
        var scheduler = new RoundRobinScheduler();
        Assert.False(scheduler.HasCommand());
    }

    [Fact]
    public void Scheduler_WithOneCommand_ShouldReturnItOnSelect()
    {
        var scheduler = new RoundRobinScheduler();
        var cmd = new TestCommand();
        
        scheduler.Add(cmd);
        Assert.True(scheduler.HasCommand());
        
        var selected = scheduler.Select();
        Assert.Same(cmd, selected);
        Assert.True(scheduler.HasCommand()); 
    }

    [Fact]
    public void RoundRobinScheduler_ShouldCycleThroughCommands()
    {
        var scheduler = new RoundRobinScheduler();
        var cmd1 = new TestCommand();
        var cmd2 = new TestCommand();
        var cmd3 = new TestCommand();

        scheduler.Add(cmd1);
        scheduler.Add(cmd2);
        scheduler.Add(cmd3);

        Assert.Same(cmd1, scheduler.Select());
        Assert.Same(cmd2, scheduler.Select());
        Assert.Same(cmd3, scheduler.Select());

        Assert.Same(cmd1, scheduler.Select());
        Assert.Same(cmd2, scheduler.Select());
    }

    [Fact]
    public void Scheduler_ShouldHandleConcurrentAddAndSelect()
    {
        var scheduler = new RoundRobinScheduler();
        var results = new ConcurrentBag<ICommand>();
        var barrier = new Barrier(3);

        var adder1 = new Thread(() =>
        {
            barrier.SignalAndWait();
            scheduler.Add(new TestCommand());
        });

        var adder2 = new Thread(() =>
        {
            barrier.SignalAndWait();
            scheduler.Add(new TestCommand());
        });

        adder1.Start();
        adder2.Start();
        barrier.SignalAndWait();

        adder1.Join();
        adder2.Join();

        Assert.True(scheduler.HasCommand());
        Assert.NotNull(scheduler.Select());
        Assert.True(scheduler.HasCommand());
    }
}
