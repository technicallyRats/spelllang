// this includes a tokenizer

using System.Collections.Generic;
using System.Linq;
using Spelllang.Parser;

namespace Spelllang.Lexer
{
    public class Lexer
    {
        private readonly string _input;

        private readonly List<LexerError> _lexerErrors = new();

        private readonly List<Token> _tokenList = new();
        private int _currentIndex;
        private int _startIndex;

        public Lexer(string input)
        {
            _input = input;
            _lex();
        }

        // TODO: adding a channel thingy probably is better...
        public TokenEnumerator GetEnumerator()
        {
            return new TokenEnumerator(_tokenList);
        }

        private void _lex()
        {
            StateFn currentStateFn = StateFnContainer.LexLine;

            do
            {
                currentStateFn = currentStateFn(this);
            } while (_tokenList.LastOrDefault().Type != Type.EOF);
        }

        public string Rest()
        {
            return _input.Substring(_currentIndex);
        }

        public void Emit(Type type)
        {
            var value = _input.Substring(_startIndex, _currentIndex - _startIndex);
            type = KeyWords.GetValueOrDefault(value, type);
            if (type == Type.UNKNOWN) ReportError("Unknown token", value.Length);
            var newToken = new Token(type, value, _startIndex);
            _tokenList.Add(newToken);
            _startIndex = _currentIndex;
        }

        public string? Peek()
        {
            if (_currentIndex >= _input.Length - 1) return null;
            return char.ToString(_input[_currentIndex + 1]);
        }

        public string? Next()
        {
            if (_currentIndex >= _input.Length - 1)
            {
                _currentIndex = _input.Length;
                return null;
            }

            return char.ToString(_input[++_currentIndex]);
        }

        public string? Current()
        {
            if (_currentIndex > _input.Length - 1) return null;
            return char.ToString(_input[_currentIndex]);
        }

        public void Ignore()
        {
            _startIndex = _currentIndex;
        }

        public bool ReachedEnd()
        {
            return Current() == null;
        }

        private void ReportError(string description, int length)
        {
            _lexerErrors.Add(new LexerError(_currentIndex, length, description));
        }

        public bool IsFaulty()
        {
            return _lexerErrors.Count > 0;
        }
    }
}