using System.Collections.Concurrent;

namespace task18;

public class RoundRobinScheduler : IScheduler
{
    private readonly ConcurrentQueue<ICommand> _queue = new();

    public bool HasCommand() => !_queue.IsEmpty;

    public ICommand Select()
    {
        if (_queue.TryDequeue(out var command)) return command;
        throw new InvalidOperationException("No commands available in scheduler");
    }

    public void Add(ICommand cmd)
    {
        ArgumentNullException.ThrowIfNull(cmd);
        _queue.Enqueue(cmd);
    }
}
