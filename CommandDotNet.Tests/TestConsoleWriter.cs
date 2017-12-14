using System;
using System.IO;
using System.Text;

namespace CommandDotNet.Tests
{
    public class TestConsoleWriter : TextWriter
    {
        public override Encoding Encoding => Encoding.UTF8;

        public override void WriteLine(string value)
        {
            WriteLineEvent?.Invoke(this, value);
            base.WriteLine(value);
        }

        public event EventHandler<string> WriteLineEvent;
    }
}