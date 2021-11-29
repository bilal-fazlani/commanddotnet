using System.IO;
using System.Text;

namespace CommandDotNet.Rendering
{
    public class DuplexTextWriter : TextWriter
    {
        public TextWriter Original { get; }
        public TextWriter Listener { get; }

        public DuplexTextWriter(TextWriter original, TextWriter listener)
        {
            Original = original;
            Listener = listener;
        }

        public override Encoding Encoding => Original.Encoding;

        public override void Write(char value)
        {
            Original.Write(value);
            Listener.Write(value);
        }
    }
}