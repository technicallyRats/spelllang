using System;
using System.Text;

namespace Spelllang.Parser
{
    public struct LexerError
    {
        public int Position;
        public int Size;
        public string Issue;

        public LexerError(int position, int size, string issue)
        {
            Position = position;
            Size = size;
            Issue = issue;
        }

        public string Show(string? sourceInput)
        {
            var sb = new StringBuilder();
            sb.Append(Issue);
            sb.Append(Environment.NewLine);

            if (sourceInput == null) return sb.ToString();

            // add the length of the issue in the front and in the back for context
            var starIdx = Math.Max(0, Position - Size);
            var leftCount = Position - starIdx;
            var endIdx = Math.Min(sourceInput.Length, Position + Size * 2);
            var rightCount = endIdx - (Position + Size);

            // Unsafe I know, i know...
            // TODO: Fix
            var sourceIssueText = sourceInput.Substring(starIdx, endIdx - starIdx);
            sb.Append(sourceIssueText);
            sb.Append(Environment.NewLine);
            sb.Append(new string(' ', leftCount));
            sb.Append(new string('^', Size));
            sb.Append(new string(' ', rightCount));

            return sb.ToString();
        }
    }
}