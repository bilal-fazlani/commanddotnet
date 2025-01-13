using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace CommandDotNet.Rendering;

[PublicAPI]
public class DuplexTextWriter(TextWriter original, TextWriter listener) : TextWriter
{
    public TextWriter Original { get; } = original;
    public TextWriter Listener { get; } = listener;

    public override Encoding Encoding => Original.Encoding;

    public override void Write(char value)
    {
        Original.Write(value);
        Listener.Write(value);
    }
}