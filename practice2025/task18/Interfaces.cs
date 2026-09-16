namespace task18;

public interface ICommand { void Execute(); }
public interface IExceptionHandler { void Handle(ICommand command, Exception exception); }
public interface IScheduler { bool HasCommand(); ICommand Select(); void Add(ICommand cmd); }
