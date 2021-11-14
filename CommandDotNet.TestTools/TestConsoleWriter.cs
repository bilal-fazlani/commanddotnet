using System.IO;
using System.Text;

namespace CommandDotNet.TestTools
{
    internal class TestConsoleWriter : TextWriter
    {
        internal TextWriter? Replaced { get; set; }
        private readonly TestConsoleWriter? _inner;
        private readonly StringBuilder _stringBuilder = new();

        public TestConsoleWriter(TestConsoleWriter? inner = null)
        {
            _inner = inner;
        }

        public override void Write(char value)
        {
            Replaced?.Write(value);
            _inner?.Write(value);
            if (value == '\b' && _stringBuilder.Length > 0)
            {
                _stringBuilder.Length -= 1;
            }
            else
            {
                _stringBuilder.Append(value);
            }
        }

        public override Encoding Encoding { get; } = Encoding.Unicode;

        public override string ToString() => _stringBuilder.ToString();
    }
}