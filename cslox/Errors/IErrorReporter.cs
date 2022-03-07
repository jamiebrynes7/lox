namespace Lox.Errors
{
    public interface IErrorReporter
    {
        bool HasError { get; }

        void Report(int line, string message);
    }
}