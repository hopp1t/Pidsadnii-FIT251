namespace task17;

public abstract class LongRunningCommand : ICommand
{
    private bool _isCompleted;

    public bool IsCompleted => _isCompleted;

    public void Execute()
    {
        if (_isCompleted) return;
        
        ExecuteStep();
        _isCompleted = IsWorkCompleted();
    }

    protected abstract void ExecuteStep();
    protected abstract bool IsWorkCompleted();
}
