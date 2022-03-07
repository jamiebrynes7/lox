namespace Lox.Errors
{
    public class ErrorReporter : IErrorReporter
    {
        public bool HasError { get; private set; }

        public void Report(int line, string message)
        {
            Console.Error.WriteLine($"[line {line}] Error: {message} ");
            HasError = true;
        }
    }
}