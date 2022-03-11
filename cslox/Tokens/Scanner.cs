using Lox.Errors;

namespace Lox.Tokens
{
    public class Scanner
    {
        private static readonly Dictionary<string, TokenType> Keywords = new()
        {
            { "and", TokenType.AND },
            { "class", TokenType.CLASS },
            { "else", TokenType.ELSE },
            { "false", TokenType.FALSE },
            { "fun", TokenType.FUN },
            { "for", TokenType.FOR },
            { "if", TokenType.IF },
            { "nil", TokenType.NIL },
            { "or", TokenType.OR },
            { "print", TokenType.PRINT },
            { "return", TokenType.RETURN },
            { "super", TokenType.SUPER },
            { "this", TokenType.THIS },
            { "true", TokenType.TRUE },
            { "var", TokenType.VAR },
            { "while", TokenType.WHILE },
        };

        private readonly Source source;
        private readonly IErrorReporter errorReporter;
        private readonly List<Token> tokens = new List<Token>();

        private int line = 1;

        public Scanner(string source, IErrorReporter errorReporter)
        {
            this.source = new Source(source);
            this.errorReporter = errorReporter;
        }

        public List<Token> ScanTokens()
        {
            while (!source.IsCompleted())
            {
                source.ResetCounters();
                ScanToken();
            }

            return tokens;
        }

        private void ScanToken()
        {
            var c = source.Advance();

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
                    AddToken(source.Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                    break;
                case '=':
                    AddToken(source.Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                    break;
                case '<':
                    AddToken(source.Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                    break;
                case '>':
                    AddToken(source.Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                    break;
                case '/':
                    if (source.Match('/'))
                    {
                        while (source.Peek() != '\n' && !source.IsCompleted())
                        {
                            source.Advance();
                        }
                    }
                    else if (source.Match('*'))
                    {
                        while (source.Peek() != '*' && source.Peek(1) != '/' && !source.IsCompleted())
                        {
                            if (source.Peek() == '\n')
                            {
                                line += 1;
                            }
                            source.Advance();
                        }

                        if (!source.IsCompleted())
                        {
                            source.Advance();
                            source.Advance();
                        }
                        else
                        {
                            errorReporter.Report(line, "Block comment not terminated");
                        }
                    }
                    else
                    {
                        AddToken(TokenType.SLASH);
                    }
                    break;
                case '"':
                    ReadString();
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
                    if (IsDigit(c))
                    {
                        ReadNumber();
                    }
                    else if (IsAlpha(c))
                    {
                        ReadIdentifier();
                    }
                    else
                    {

                        errorReporter.Report(line, "Unexpected character.");
                    }
                    break;
            }
        }

        private void ReadString()
        {
            while (source.Peek() != '"' && !source.IsCompleted())
            {
                if (source.Peek() == '\n')
                {
                    line++;
                }

                source.Advance();
            }

            if (source.IsCompleted())
            {
                errorReporter.Report(line, "Unterminated string.");
                return;
            }

            // The closing '"'
            source.Advance();

            // Trim the surrounding quotes
            var str = source.GetRawToken(1, 1);
            AddToken(TokenType.STRING, str);
        }

        private void ReadNumber()
        {
            while (IsDigit(source.Peek()))
            {
                source.Advance();
            }

            // Looking for a fractional part.
            if (source.Peek() == '.' && IsDigit(source.Peek(1)))
            {
                // Skip the '.'
                source.Advance();
            }

            while (IsDigit(source.Peek()))
            {
                source.Advance();
            }

            AddToken(TokenType.NUMBER, double.Parse(source.GetRawToken()));
        }

        private void ReadIdentifier()
        {
            while (IsAlphaNumeric(source.Peek()))
            {
                source.Advance();
            }

            var text = source.GetRawToken();

            if (!Keywords.TryGetValue(text, out var tokenType))
            {
                tokenType = TokenType.IDENTIFIER;
            }

            AddToken(tokenType);
        }

        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        private void AddToken(TokenType type, object? literal)
        {
            tokens.Add(new Token(type, source.GetRawToken(), literal, line));
        }

        private static bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private static bool IsAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';
        }

        private static bool IsAlphaNumeric(char c)
        {
            return IsDigit(c) || IsAlpha(c);
        }

        private class Source
        {
            private int start = 0;
            private int current = 0;

            private string source;

            public Source(string source)
            {
                this.source = source;
            }

            public char Advance()
            {
                return source[current++];
            }

            public bool IsCompleted()
            {
                return current >= source.Length;
            }

            public char Peek(int offset = 0)
            {
                var idx = current + offset;
                if (idx >= source.Length)
                {
                    return '\0';
                }

                return source[idx];
            }

            public bool Match(char expected)
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

            public string GetRawToken()
            {
                return source.Substring(start, current - start);
            }

            public string GetRawToken(int startOffset, int endOffset)
            {
                return source.Substring(start + startOffset, current - start - endOffset);
            }

            public void ResetCounters()
            {
                start = current;
            }
        }
    }

}