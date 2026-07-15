using System.Collections.Concurrent;

namespace task17;

public class RoundRobinScheduler : IScheduler
{
    private readonly ConcurrentQueue<ICommand> _queue = new();
    private readonly object _lock = new();

    public bool HasCommand()
    {
        return !_queue.IsEmpty;
    }

    public ICommand Select()
    {
        if (_queue.TryDequeue(out var command))
        {
            _queue.Enqueue(command);
            return command;
        }
        throw new InvalidOperationException("No commands available");
    }

    public void Add(ICommand cmd)
    {
        ArgumentNullException.ThrowIfNull(cmd);
        _queue.Enqueue(cmd);
    }
}
