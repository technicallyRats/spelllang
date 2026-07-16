using System;
using System.Text;

namespace Spelllang.Parser
{
    public struct ParsingError
    {
        public long Position;
        public int Size;
        public string Issue;

        public ParsingError(long position, int size, string issue)
        {
            Position = position;
            Size = size;
            Issue = issue;
        }

        public string Show(string sourceInput)
        {
            var sb = new StringBuilder();
            sb.Append(Issue);
            sb.Append("\n");

            // add the length of the issue in the front and in the back for context
            var starIdx = Math.Max(0, Position - Size);
            var endIdx = Math.Min(sourceInput.Length, Position + Size * 2);

            // Unsafe I know, i know...
            // TODO: Fix
            var sourceIssueText = sourceInput.Substring((int)starIdx, (int)(endIdx - starIdx));
            sb.Append(sourceIssueText);

            return sb.ToString();
        }
    }
}