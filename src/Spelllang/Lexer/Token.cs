using System.Collections.Generic;
using System.Linq;

namespace Spelllang.Lexer
{
    public enum Type
    {
        EMPTY,
        EOF,
        IDENTIFIER,
        ASSIGN,
        EQUAL,
        NOT_EQUAL,
        NOT,
        GT,
        GTE,
        LT,
        LTE,
        PLUS,
        MINUS,
        MULTIPLY,
        DIVIDE,
        MODULO,
        PARENTHESES_LEFT,
        PARENTHESES_RIGHT,
        BRACES_LEFT,
        BRACES_RIGHT,
        COMMA,
        NUMBER,
        STRING,
        SEMICOLON,
        BOOLEAN,
        FUNCTION,
        RETURN,
        NULL,
        AND,
        OR,

        IF,
        ELSE,

        WHILE,
        BREAK,

        IMPORT,
        AS,

        UNKNOWN
    }

    public static class KeyWords
    {
        private static readonly Dictionary<string, Type> ReservedKeyWords = new()
        {
            { "true", Type.BOOLEAN },
            { "false", Type.BOOLEAN },
            { "function", Type.FUNCTION },
            { "return", Type.RETURN },
            { "null", Type.NULL },
            { "if", Type.IF },
            { "else", Type.ELSE },
            { "while", Type.WHILE },
            { "break", Type.BREAK },
            { "import", Type.IMPORT },
            { "as", Type.AS }
        };

        public static Type GetValueOrDefault(string keyword, Type fallback)
        {
            return ReservedKeyWords.TryGetValue(keyword, out var value) ? value : fallback;
        }
    }

    public struct Token
    {
        public Token(Type type, string value, int startIndex)
        {
            Type = type;
            Value = value;
            StartIndex = startIndex;
        }

        public int StartIndex;

        public Type Type { get; }
        public string Value { get; }

        public override string ToString()
        {
            return Type + "->" + Value;
        }
    }

// custom enumerator like implementation to support peek
    public class TokenEnumerator
    {
        private readonly List<Token> _tokenList;

        public TokenEnumerator(List<Token> tokenList)
        {
            _tokenList = tokenList;
        }

        private int CurrentIndex { get; set; }

        private int GetMaxValidIndex()
        {
            return _tokenList.Last().StartIndex + _tokenList.Last().Value.Length;
        }

        public Token Current()
        {
            if (CurrentIndex > _tokenList.Count - 1)
                return new Token(Type.EOF, "", GetMaxValidIndex());

            return _tokenList.ElementAt(CurrentIndex);
        }

        public Token Next()
        {
            if (CurrentIndex >= _tokenList.Count - 1)
            {
                CurrentIndex++;
                return new Token(Type.EOF, "", GetMaxValidIndex());
            }

            return _tokenList[++CurrentIndex];
        }

        public Token Peek()
        {
            if (CurrentIndex >= _tokenList.Count - 1)
                return new Token(Type.EOF, "", GetMaxValidIndex());

            return _tokenList[CurrentIndex + 1];
        }
    }
}