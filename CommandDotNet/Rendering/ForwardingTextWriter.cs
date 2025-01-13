using System;
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace CommandDotNet.Rendering;

[PublicAPI]
public class ForwardingTextWriter(Action<string?> write) : TextWriter
{
    public override Encoding Encoding { get; } = Encoding.Default;

    public override void Write(string? value) => write(value);

    public override void Write(char value) => write(value.ToString());
}