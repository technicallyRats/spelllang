namespace Spelllang.Tests.TestUtils
{
    public class Incrementer
    {
        private int index;

        public int Increment(string v)
        {
            index += v.Length;
            return index - v.Length;
        }

        public void Reset()
        {
            index = 0;
        }
    }
}