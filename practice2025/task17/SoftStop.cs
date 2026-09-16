using System.Threading;

namespace task17;

public class SoftStop : ICommand
{
    private readonly int _targetThreadId;

    public SoftStop(int targetThreadId)
    {
        _targetThreadId = targetThreadId;
    }

    public void Execute()
    {
        if (Thread.CurrentThread.ManagedThreadId != _targetThreadId)
        {
            throw new InvalidOperationException(
                $"SoftStop can only be successfully executed in its target thread (id {_targetThreadId}).");
        }
    }
}
