using System.Text.RegularExpressions;

namespace Spelllang.Lexer
{
    public delegate StateFn StateFn(Lexer lexer);

    public class StateFnContainer
    {
        private static readonly string ALLOWED_IDENTIFIER_START_CHARACTERS = @"^[a-zA-Z_]+$";

        private static readonly string NUMBER_INITIAL = @"^[0-9]+$";

        private static readonly string ALLOWED_IDENTIFIER_CHARACTERS = @"^[a-zA-Z0-9_\.]+$";

        public static StateFn LexLine(Lexer lexer)
        {
            if (lexer.ReachedEnd())
            {
                lexer.Emit(Type.EOF);
                lexer.Next();
                return LexLine;
            }

            if (Regex.IsMatch(lexer.Current(), ALLOWED_IDENTIFIER_START_CHARACTERS)) return LexIdentifier;

            if (Regex.IsMatch(lexer.Current(), NUMBER_INITIAL)) return LexNumber;

            switch (lexer.Current())
            {
                case " ":
                case "\n":
                case "\r":
                    lexer.Next();
                    lexer.Ignore();
                    break;
                case "'":
                    return LexString;
                case ";":
                    return BuildSingleEmitStateFn(Type.SEMICOLON);
                case ",":
                    return BuildSingleEmitStateFn(Type.COMMA);
                case "(":
                    return BuildSingleEmitStateFn(Type.PARENTHESES_LEFT);
                case ")":
                    return BuildSingleEmitStateFn(Type.PARENTHESES_RIGHT);
                case "+":
                    return BuildSingleEmitStateFn(Type.PLUS);
                case "-":
                    return BuildSingleEmitStateFn(Type.MINUS);
                case "*":
                    return BuildSingleEmitStateFn(Type.MULTIPLY);
                case "/":
                    return BuildSingleEmitStateFn(Type.DIVIDE);
                case "%":
                    return BuildSingleEmitStateFn(Type.MODULO);
                case "{":
                    return BuildSingleEmitStateFn(Type.BRACES_LEFT);
                case "}":
                    return BuildSingleEmitStateFn(Type.BRACES_RIGHT);
                case "=":
                    return BuildConditionalEmitStateFn("=", Type.EQUAL, Type.ASSIGN);
                case "!":
                    return BuildConditionalEmitStateFn("=", Type.NOT_EQUAL, Type.NOT);
                case ">":
                    return BuildConditionalEmitStateFn("=", Type.GTE, Type.GT);
                case "<":
                    return BuildConditionalEmitStateFn("=", Type.LTE, Type.LT);
                case "&":
                    return BuildConditionalEmitStateFn("&", Type.AND, Type.UNKNOWN);
                case "|":
                    return BuildConditionalEmitStateFn("|", Type.OR, Type.UNKNOWN);
                default:
                    lexer.Emit(Type.UNKNOWN);
                    lexer.Next();
                    break;
            }

            return LexLine;
        }

        public static StateFn BuildSingleEmitStateFn(Type type)
        {
            return lexer =>
            {
                lexer.Next();
                lexer.Emit(type);
                return LexLine;
            };
        }

        public static StateFn BuildConditionalEmitStateFn(string conditional, Type onMatchType, Type noMatchType)
        {
            return lexer =>
            {
                lexer.Next();
                if (lexer.Current() == conditional)
                {
                    lexer.Next();
                    lexer.Emit(onMatchType);
                }
                else
                {
                    lexer.Emit(noMatchType);
                }

                return LexLine;
            };
        }

        public static StateFn LexString(Lexer lexer)
        {
            var terminatingCharacter = lexer.Current();
            lexer.Next();
            lexer.Ignore();
            while (lexer.Current() != terminatingCharacter) lexer.Next();
            lexer.Emit(Type.STRING);
            lexer.Next();
            lexer.Ignore();
            return LexLine;
        }

        public static StateFn LexIdentifier(Lexer lexer)
        {
            while (Regex.IsMatch(lexer.Current(), ALLOWED_IDENTIFIER_CHARACTERS))
                if (lexer.Next() == null)
                    break;

            lexer.Emit(Type.IDENTIFIER);
            return LexLine;
        }

        public static StateFn LexNumber(Lexer lexer)
        {
            while (
                lexer.Current() != null &&
                // is number
                Regex.IsMatch(lexer.Current(), NUMBER_INITIAL)
            )
            {
                lexer.Next();

                // number is format 1_000 or 3.41
                if ((lexer.Current() == "_" || lexer.Current() == ".") && Regex.IsMatch(lexer.Peek(), NUMBER_INITIAL))
                    lexer.Next();
            }

            lexer.Emit(Type.NUMBER);
            return LexLine;
        }
    }
}