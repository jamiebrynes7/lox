using Lox.Errors;

namespace Lox.Tokens
{
    public class Scanner
    {
        private readonly string source;
        private readonly IErrorReporter errorReporter;
        private readonly List<Token> tokens = new List<Token>();

        private int start = 0;
        private int current = 0;
        private int line = 1;

        public Scanner(string source, IErrorReporter errorReporter)
        {
            this.source = source;
            this.errorReporter = errorReporter;
        }

        public List<Token> ScanTokens()
        {
            while (!IsCompleted())
            {
                start = current;
                ScanToken();
            }

            return tokens;
        }

        private void ScanToken()
        {
            var c = Advance();

            switch (c)
            {
                case '(':
                    AddToken(TokenType.LEFT_PAREN);
                    break;
                case ')':
                    AddToken(TokenType.RIGHT_PAREN);
                    break;
                case '{':
                    AddToken(TokenType.LEFT_BRACE);
                    break;
                case '}':
                    AddToken(TokenType.RIGHT_BRACE);
                    break;
                case ',':
                    AddToken(TokenType.COMMA);
                    break;
                case '.':
                    AddToken(TokenType.DOT);
                    break;
                case '-':
                    AddToken(TokenType.MINUS);
                    break;
                case '+':
                    AddToken(TokenType.PLUS);
                    break;
                case ';':
                    AddToken(TokenType.SEMICOLON);
                    break;
                case '*':
                    AddToken(TokenType.STAR);
                    break;
                case '!':
                    AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                    break;
                case '=':
                    AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                    break;
                case '<':
                    AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                    break;
                case '>':
                    AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                    break;
                case '/':
                    if (Match('/'))
                    {
                        while ($"{Peek()}" != Environment.NewLine && !IsCompleted())
                        {
                            Advance();
                        }
                    }
                    else
                    {
                        AddToken(TokenType.SLASH);
                    }
                    break;
                case ' ':
                case '\r':
                case '\t':
                    // Ignore whitespace
                    break;
                case '\n':
                    line++;
                    break;
                default:
                    errorReporter.Report(line, "Unexpected character.");
                    break;
            }
        }

        private bool IsCompleted()
        {
            return false;
        }

        private char Advance()
        {
            return source[current++];
        }

        private bool Match(char expected)
        {
            if (IsCompleted())
            {
                return false;
            }

            if (source[current] != expected)
            {
                return false;
            }

            current++;
            return true;
        }

        private char Peek()
        {
            if (IsCompleted())
            {
                return '\0';
            }

            return source[current];
        }

        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        private void AddToken(TokenType type, object? literal)
        {
            var text = source.Substring(start, current);
            tokens.Add(new Token(type, text, literal, line));
        }
    }
}