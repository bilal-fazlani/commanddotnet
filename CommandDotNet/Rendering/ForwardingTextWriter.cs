using System;
using System.IO;
using System.Text;

namespace CommandDotNet.Rendering
{
    public class ForwardingTextWriter : TextWriter
    {
        private Action<string?> _write;

        public ForwardingTextWriter(Action<string?> write)
        {
            _write = write;
        }

        public override Encoding Encoding { get; }

        public override void Write(string value)
        {
            _write(value);
        }

        public override void Write(char value)
        {
            _write(value.ToString());
        }
    }
}