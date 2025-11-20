using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace CommandDotNet.Rendering;

/// <summary>
/// A TextWriter that duplicates all writes to two destinations simultaneously.
/// </summary>
/// <remarks>
/// Purpose: Enable output to be sent to multiple sinks in a single write operation.
/// 
/// Usage Pattern:
/// - Original: The primary destination (e.g., ForwardingTextWriter to Spectre's IAnsiConsole)
/// - Listener: The secondary destination (e.g., TestConsoleWriter for test capture)
/// 
/// Data Flow:
/// When Write() is called → writes to BOTH Original and Listener
/// 
/// Example Use Case in Testing:
/// When using AnsiTestConsole, Console.Out writes need to go to:
/// 1. Spectre's IAnsiConsole (for rendering) via ForwardingTextWriter
/// 2. TestConsoleWriter (for test assertions) via base.Out
/// 
/// Note: ToString() delegates to Listener because that's typically the capture sink
/// that accumulates output for retrieval (e.g., TestConsoleWriter has StringBuilder).
/// Original is often a ForwardingTextWriter that doesn't accumulate text.
/// </remarks>
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

    /// <summary>
    /// Returns the accumulated text from the Listener writer.
    /// The Listener is typically a capturing writer like TestConsoleWriter.
    /// </summary>
    public override string ToString() => Listener.ToString() ?? string.Empty;
}