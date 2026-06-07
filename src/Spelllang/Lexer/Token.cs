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
        PARENTHESES_LEFT,
        PARENTHESES_RIGHT,
        COMMA,
        NUMBER,
        STRING,
        SEMICOLON,
        BOOLEAN,
        UNKNOWN
    }

    public static class KeyWords
    {
        private static readonly Dictionary<string, Type> ReservedKeyWords = new Dictionary<string, Type>
        {
            { "true", Type.BOOLEAN },
            { "false", Type.BOOLEAN }
        };

        public static Type GetValueOrDefault(string keyword, Type fallback)
        {
            return ReservedKeyWords.TryGetValue(keyword, out Type value) ? value : fallback;
        }

    }

    public struct Token
    {
        public Token(Type type, string value)
        {
            Type = type;
            Value = value;
        }

        public Type Type { get; }
        public string Value { get; }

        public string ToString()
        {
            return Type + "->" + Value;
        }
    }

    // custom enumerator like implementation to support peek
    public class TokenEnumerator
    {
        private List<Token> TokenList;
        private int CurrentIndex = 0;

        public TokenEnumerator(List<Token> tokenList)
        {
            TokenList = tokenList;
        }

        public Token Current()
        {
            if (CurrentIndex >= TokenList.Count)
            {
                return new Token(Type.EOF, "");
            }

            return TokenList.ElementAt(CurrentIndex);
        }

        public Token Next()
        {
            if (CurrentIndex >= TokenList.Count - 1)
            {
                return new Token(Type.EOF, "");
            }

            return TokenList[++CurrentIndex];
        }

        public Token Peek()
        {
            if (CurrentIndex >= TokenList.Count - 1)
            {
                return new Token(Type.EOF, "");
            }

            return TokenList[CurrentIndex + 1];
        }

        public bool MoveNext()
        {
            if (Next().Type == Type.EOF)
            {
                return false;
            }

            return CurrentIndex >= TokenList.Count - 1;
        }
    }
}
