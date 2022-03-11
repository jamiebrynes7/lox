using Lox.Errors;
using Lox.Tokens;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Usage: cslox [script]");
            Environment.Exit(1);
        }
        else if (args.Length == 1)
        {
            RunFile(args[0]);
        }
        else
        {
            RunPrompt();
        }

    }

    private static void RunFile(string filePath)
    {
        var program = File.ReadAllText(filePath);
        var reporter = new ErrorReporter();

        Run(program, reporter);

        if (reporter.HasError)
        {
            Environment.Exit(65);
        }
    }

    private static void RunPrompt()
    {
        while (true)
        {
            Console.Write("> ");
            var line = Console.ReadLine();

            if (line == null)
            {
                break;
            }

            Run(line, new NoOpErrorReporter());
        }
    }

    private static void Run(string program, IErrorReporter errorReporter)
    {
        var scanner = new Scanner(program, errorReporter);

        foreach (var token in scanner.ScanTokens())
        {
            Console.WriteLine(token);
        }
    }
}

