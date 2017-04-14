using System;

namespace Lexer
{
    class Token
    {
        private TokenType m_Type;
        private string m_Lexeme;
        private object m_Literal;
        private int m_Line;

        internal Token(TokenType type, String lexeme, Object literal, int line)
        {
            this.m_Type = type;
            this.m_Lexeme = lexeme;
            this.m_Literal = literal;
            this.m_Line = line;
        }

        public override string ToString()
        {
            return m_Type + " " + m_Lexeme + " " + m_Literal;
        }
    }
}
