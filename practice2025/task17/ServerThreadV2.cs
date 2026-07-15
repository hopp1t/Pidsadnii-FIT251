using System.Collections.Concurrent;
using System.Threading;

namespace task17;

public class ServerThreadV2
{
    private readonly BlockingCollection<ICommand> _commandQueue = new();
    private readonly IScheduler _scheduler;
    private readonly IExceptionHandler _exceptionHandler;
    private Thread? _thread;
    private int _threadId;
    private bool _isSoftStopping;
    private readonly ManualResetEventSlim _startedEvent = new();
    private readonly object _schedulerLock = new();

    public ServerThreadV2(
        IScheduler? scheduler = null,
        IExceptionHandler? exceptionHandler = null)
    {
        _scheduler = scheduler ?? new RoundRobinScheduler();
        _exceptionHandler = exceptionHandler ?? new DefaultExceptionHandler();
    }

    public bool IsAlive => _thread?.IsAlive == true;

    public void Start()
    {
        if (_thread != null && _thread.IsAlive)
            throw new InvalidOperationException("ServerThreadV2 is already running.");

        _isSoftStopping = false;
        _thread = new Thread(ThreadProc) { IsBackground = true };
        _thread.Start();
        _startedEvent.Wait();
    }

    public void Enqueue(ICommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        _commandQueue.Add(command);
    }

    public void EnqueueHardStop()
    {
        Enqueue(new HardStop(_threadId));
    }

    public void EnqueueSoftStop()
    {
        Enqueue(new SoftStop(_threadId));
    }

    public void Join()
    {
        _thread?.Join();
    }

    public bool Join(int millisecondsTimeout)
    {
        return _thread?.Join(millisecondsTimeout) ?? true;
    }

    private void ThreadProc()
    {
        _threadId = Thread.CurrentThread.ManagedThreadId;
        _startedEvent.Set();

        while (true)
        {
            ICommand? commandToExecute = null;

            if (_commandQueue.TryTake(out commandToExecute!))
            {
                
            }
            else if (_scheduler.HasCommand())
            {
                lock (_schedulerLock)
                {
                    if (_scheduler.HasCommand())
                    {
                        try
                        {
                            commandToExecute = _scheduler.Select();
                        }
                        catch (Exception ex)
                        {
                            _exceptionHandler.Handle(null!, ex);
                            continue;
                        }
                    }
                }
            }

            if (commandToExecute == null)
            {
                try
                {
                    commandToExecute = _commandQueue.Take();
                }
                catch (InvalidOperationException)
                {
                    break;
                }
            }

            if (commandToExecute == null) continue;

            try
            {
                commandToExecute.Execute();

                if (commandToExecute is HardStop)
                {
                    if (Thread.CurrentThread.ManagedThreadId == _threadId)
                    {
                        _commandQueue.CompleteAdding();
                        break;
                    }
                    else
                    {
                        throw new InvalidOperationException("HardStop executed in wrong thread");
                    }
                }

                if (commandToExecute is SoftStop)
                {
                    if (Thread.CurrentThread.ManagedThreadId == _threadId)
                    {
                        _isSoftStopping = true;
                    }
                    else
                    {
                        throw new InvalidOperationException("SoftStop executed in wrong thread");
                    }
                }

                if (commandToExecute is LongRunningCommand longCmd && !longCmd.IsCompleted)
                {
                    lock (_schedulerLock)
                    {
                        _scheduler.Add(longCmd);
                    }
                }

                if (_isSoftStopping && !_scheduler.HasCommand() && _commandQueue.Count == 0)
                {
                    break;
                }
            }
            catch (Exception ex)
            {
                _exceptionHandler.Handle(commandToExecute, ex);
            }
        }
    }
}
