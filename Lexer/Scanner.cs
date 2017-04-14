using System;
using System.Collections.Generic;

namespace Lexer
{
    internal class Scanner
    {
        private string m_Source;
        private List<Token> m_Tokens = new List<Token>();
        private int m_StartChar = 0;
        private int m_CurrentChar = 0;
        private int m_CurrentLine = 0;


        internal Scanner(string source)
        {
            this.m_Source = source;
        }

        internal List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                // We are at the beginning of the next lexeme.
                m_StartChar = m_CurrentChar;
                ScanToken();
            }


            m_Tokens.Add(new Token(TokenType.EOF, "", null, m_CurrentLine));
            return m_Tokens;
        }

        internal void ScanToken()
        {
            char c = Advance();
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
                        while (Peek() != '\n' && !IsAtEnd()) // A comment goes until the end of the line.
                            Advance();
                    }
                    else
                    {
                        AddToken(TokenType.SLASH);
                    }
                    break;
                // Ignore whitespaces
                case ' ':
                case '\r':
                case '\t':
                    break;
                case '\n':
                    m_CurrentLine += 1;
                    break;
                case '"':
                    GetString();
                    break;
                default:
                    if (char.IsDigit(c))
                    {
                        GetNumber();
                    }
                    else if (char.IsLetter(c))
                    {
                        GetIdentifier();
                    }
                    else
                    {
                        Error(m_CurrentLine, "Unrecognized token");
                    }
                    
                    break;
            }
        }

        private void GetIdentifier()
        {
            while (char.IsLetterOrDigit(Peek()))
                Advance();

            AddToken(TokenType.IDENTIFIER);
        }

        //valid : 1234
        //valid : 12.34
        //invalid : .1234
        //invalid : 1234.
        private void GetNumber()
        {
            int length = 0;
            while (Char.IsDigit(Peek()))
            {
                Advance();
                length += 1;
            }

            // Look for a fractional part.
            if (Peek() == '.' && Char.IsDigit(PeekNext()))
            {
                Advance();

                while (Char.IsDigit(Peek()))
                {
                    Advance();
                    length += 1;
                }
            }

            AddToken(TokenType.NUMBER, double.Parse(m_Source.Substring(m_StartChar, length+1)));
        }

        private char PeekNext()
        {
            if (m_CurrentChar + 1 >= m_Source.Length)
                return '\0';
            return m_Source[m_CurrentChar + 1];
        }

        //allows multilangauge strings
        private void GetString()
        {
            int length = 0;
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n')
                    m_CurrentLine += 1;

                length += 1;
                Advance();
            }

            // Unterminated string.
            if (IsAtEnd())
            {
                Error(m_CurrentLine, "Unterminated string.");
                return;
            }

            // The closing ".
            Advance();

            // Trim the surrounding quotes.
            string value = m_Source.Substring(m_StartChar + 1, length);
            AddToken(TokenType.STRING, value);
        }

        private Dictionary<string, TokenType> GetKeywords()
        {
            Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>();
            keywords.Add("and", TokenType.AND);
            keywords.Add("class", TokenType.CLASS);
            keywords.Add("else", TokenType.ELSE);
            keywords.Add("false", TokenType.FALSE);
            keywords.Add("for", TokenType.FOR);
            keywords.Add("fun", TokenType.FUN);
            keywords.Add("if", TokenType.IF);
            keywords.Add("nil", TokenType.NIL);
            keywords.Add("or", TokenType.OR);
            keywords.Add("print", TokenType.PRINT);
            keywords.Add("return", TokenType.RETURN);
            keywords.Add("super", TokenType.SUPER);
            keywords.Add("this", TokenType.THIS);
            keywords.Add("true", TokenType.TRUE);
            keywords.Add("var", TokenType.VAR);
            keywords.Add("while", TokenType.WHILE);

            return keywords;
        }

        //one character lookahead method
        private char Peek()
        {
            if (m_CurrentChar >= m_Source.Length)
                return '\0';
            return m_Source[m_CurrentChar];
        }

        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        private void AddToken(TokenType type, Object literal)
        {
            String text = m_Source.Substring(m_StartChar, m_CurrentChar - m_StartChar);
            m_Tokens.Add(new Token(type, text, literal, m_CurrentLine));
        }


        private bool IsAtEnd()
        {
            return m_CurrentChar >= m_Source.Length;
        }

        private char Advance()
        {
            m_CurrentChar += 1;
            return m_Source[m_CurrentChar - 1];
        }

        private bool Match(char expected)
        {
            if (IsAtEnd())
                return false;
            if (m_Source[m_CurrentChar] != expected)
                return false;

            m_CurrentChar+=1;
            return true;
        }

        static internal void Error(int line, string message)
        {
            Console.WriteLine("Line:" + line + "Message:" + message);
        }
    }
}
