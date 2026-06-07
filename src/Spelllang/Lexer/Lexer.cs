// this includes a tokenizer

using System;
using System.Collections.Generic;
using System.Linq;

namespace Spelllang.Lexer
{
    public class Lexer
    {
        private int StartIndex = 0;
        private int CurrentIndex = 0;

        private string Input;

        private List<Token> TokenList = new List<Token>();

        public Lexer(string input)
        {
            Input = input;
            _lex();
        }

        // TODO: adding a channel thingy probably is better...
        public TokenEnumerator GetEnumerator()
        {
            return new TokenEnumerator(TokenList);
        }

        private void _lex()
        {
            StateFn currentStateFn = StateFnContainer.LexLine;

            do
            {
                currentStateFn = currentStateFn(this);
            }
            while (TokenList.LastOrDefault().Type != Type.EOF);
        }

        public string Rest()
        {
            return Input.Substring(CurrentIndex);
        }

        public void Emit(Type type)
        {
            string value = Input.Substring(StartIndex, CurrentIndex - StartIndex);
            type = KeyWords.GetValueOrDefault(value, type);
            Token newToken = new Token(type, value);
            TokenList.Add(newToken);
            StartIndex = CurrentIndex;
        }

        public string Peek()
        {
            if (CurrentIndex >= Input.Length - 1)
            {
                return null;
            }
            return Char.ToString(Input[CurrentIndex + 1]);
        }

        public string Next()
        {
            if (CurrentIndex >= Input.Length - 1)
            {
                CurrentIndex = Input.Length;
                return null;
            }
            return Char.ToString(Input[++CurrentIndex]);
        }

        public string Current()
        {
            if (CurrentIndex > Input.Length - 1)
            {
                return null;
            }
            return Char.ToString(Input[CurrentIndex]);
        }

        public void Ignore()
        {
            StartIndex = CurrentIndex;
        }

        public bool ReachedEnd()
        {
            return Current() == null;
        }
    }
}
