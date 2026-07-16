using task18;
using Xunit;

namespace task18tests;

public class ExceptionHandlerTests
{
    private class TestCommand : ICommand
    {
        public void Execute() => throw new InvalidOperationException("Test exception");
    }

    [Fact]
    public void DefaultExceptionHandler_ShouldNotThrow()
    {
        var handler = new DefaultExceptionHandler();
        var command = new TestCommand();
        var exception = new InvalidOperationException("Test");
        
        var ex = Record.Exception(() => handler.Handle(command, exception));
        Assert.Null(ex);
    }

    [Fact]
    public void CustomExceptionHandler_ShouldReceiveCommandAndException()
    {
        ICommand? receivedCommand = null;
        Exception? receivedException = null;

        var handler = new CustomHandler((cmd, ex) =>
        {
            receivedCommand = cmd;
            receivedException = ex;
        });

        var command = new TestCommand();
        var exception = new InvalidOperationException("Test");
        handler.Handle(command, exception);

        Assert.Same(command, receivedCommand);
        Assert.Same(exception, receivedException);
    }

    private class CustomHandler : IExceptionHandler
    {
        private readonly Action<ICommand, Exception> _action;
        public CustomHandler(Action<ICommand, Exception> action) => _action = action;
        public void Handle(ICommand command, Exception exception) => _action(command, exception);
    }
}
