using System.Threading;

namespace task18;

public class DefaultExceptionHandler : IExceptionHandler 
{ 
    public void Handle(ICommand command, Exception exception) { } 
}

public class HardStop : ICommand 
{
    private readonly int _targetThreadId;
    public HardStop(int targetThreadId) => _targetThreadId = targetThreadId;
    public void Execute() 
    { 
        if (Thread.CurrentThread.ManagedThreadId != _targetThreadId) 
            throw new InvalidOperationException("HardStop executed in wrong thread"); 
    }
}

public class SoftStop : ICommand 
{
    private readonly int _targetThreadId;
    public SoftStop(int targetThreadId) => _targetThreadId = targetThreadId;
    public void Execute() 
    { 
        if (Thread.CurrentThread.ManagedThreadId != _targetThreadId) 
            throw new InvalidOperationException("SoftStop executed in wrong thread"); 
    }
}

public abstract class LongRunningCommand : ICommand 
{
    private bool _isCompleted;
    public bool IsCompleted => _isCompleted;

    public void Execute() 
    { 
        if (!_isCompleted) 
        { 
            ExecuteStep(); 
            _isCompleted = IsWorkCompleted(); 
        } 
    }
    protected abstract void ExecuteStep();
    protected abstract bool IsWorkCompleted();
}
