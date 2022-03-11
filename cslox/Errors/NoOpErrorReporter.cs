namespace Lox.Errors
{
    public class NoOpErrorReporter : IErrorReporter
    {
        public bool HasError { get; } = false;

        public void Report(int line, string message)
        {
            Console.Error.WriteLine($"[line {line}] Error: {message} ");
        }
    }
}