using System.Collections.Concurrent;
using System.Threading;

namespace task17;

public class ServerThread
{
    private readonly BlockingCollection<ICommand> _queue = new();
    private readonly IExceptionHandler _exceptionHandler;
    private Thread? _thread;
    private int _threadId;
    private bool _isSoftStopping;
    private readonly ManualResetEventSlim _startedEvent = new();

    public ServerThread(IExceptionHandler? exceptionHandler = null)
    {
        _exceptionHandler = exceptionHandler ?? new DefaultExceptionHandler();
    }

    public void Start()
    {
        if (_thread != null && _thread.IsAlive)
            throw new InvalidOperationException("ServerThread is already running.");

        _isSoftStopping = false;
        _thread = new Thread(ThreadProc) { IsBackground = true };
        _thread.Start();
        _startedEvent.Wait();
    }

    public void Enqueue(ICommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        _queue.Add(command);
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

    private void ThreadProc()
    {
        _threadId = Thread.CurrentThread.ManagedThreadId;
        _startedEvent.Set();

        while (true)
        {
            ICommand command;
            try
            {
                if (_isSoftStopping)
                {
                    if (!_queue.TryTake(out command!, 50))
                    {
                        if (_queue.Count == 0) break;
                        continue;
                    }
                }
                else
                {
                    command = _queue.Take();
                }
            }
            catch (InvalidOperationException)
            {
                break;
            }

            try
            {
                command.Execute();

                if (command is HardStop)
                {
                    _queue.CompleteAdding();
                    break;
                }

                if (command is SoftStop)
                {
                    _isSoftStopping = true;
                }

                if (_isSoftStopping && _queue.Count == 0)
                {
                    break;
                }
            }
            catch (Exception ex)
            {
                _exceptionHandler.Handle(command, ex);
            }
        }
    }
}
