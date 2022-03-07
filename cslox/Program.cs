using Lox.Errors;

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
        // run(program, reporter)

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

            Console.WriteLine(line);

            if (line == null)
            {
                break;
            }

            // run(line)
        }
    }

    private static void Run(string program)
    {
        /*
        var scanner = new Scanner(program);

        foreach (var token in scanner.scanTokens()) {
            Console.WriteLine(token);
        }
        */
    }
}

