using System.IO;

namespace CommandDotNet.Tests.Utils
{
    public class TestWriter
    {
        private TextWriter _out;

        public TestWriter(TextWriter @out)
        {
            _out = @out;
        }

        public void Write(string text)
        {
            _out.Write(text);
        }
    }
}