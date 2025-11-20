using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using CommandDotNet.Rendering;
using Spectre.Console;

[assembly: InternalsVisibleTo("CommandDotNet.Spectre.Testing")]

namespace CommandDotNet.Spectre;

/// <summary>
/// An IConsole implementation that forwards Console.Out writes to Spectre's IAnsiConsole
/// for rich console rendering support.
/// </summary>
/// <remarks>
/// Purpose: Bridge between CommandDotNet's IConsole and Spectre.Console's IAnsiConsole,
/// enabling CommandDotNet apps to use Spectre's rich rendering features.
/// 
/// Two Usage Scenarios:
/// 
/// 1. PRODUCTION (public constructor):
///    - Console.Out → ForwardingTextWriter → IAnsiConsole.Write()
///    - Output goes directly to Spectre for rendering
///    - No capture needed
/// 
/// 2. TESTING (internal constructor via AnsiTestConsole):
///    - Console.Out → DuplexTextWriter →
///      a) ForwardingTextWriter → Spectre.Console.Testing.TestConsole (for Spectre features)
///      b) TestConsoleWriter → base.Out (for test assertions via OutText())
///    - Output is duplicated to both destinations:
///      * Spectre's TestConsole.Output (accessed via SpectreTestConsole.Output)
///      * TestConsoleWriter (accessed via base.Out.ToString() or OutText())
///    - AnsiTestConsole.OutText() combines output from both sources
/// </remarks>
public class AnsiConsoleForwardingConsole : SystemConsole
{
    private readonly IAnsiConsole _ansiConsole;

    /// <summary>
    /// Creates a console for production use where Console.Out writes directly to Spectre.
    /// </summary>
    public AnsiConsoleForwardingConsole(IAnsiConsole ansiConsole)
    {
        _ansiConsole = ansiConsole;
        Out = new ForwardingTextWriter(ansiConsole.Write!);
    }

    /// <summary>
    /// Creates a console for testing where Console.Out writes to BOTH Spectre and an additional capture writer.
    /// Used by AnsiTestConsole to enable output assertions while preserving Spectre functionality.
    /// </summary>
    /// <param name="ansiConsole">Spectre's TestConsole for Spectre-specific rendering</param>
    /// <param name="additionalWriter">TestConsoleWriter (base.Out) for capturing output for test assertions</param>
    internal AnsiConsoleForwardingConsole(IAnsiConsole ansiConsole, TextWriter additionalWriter)
    {
        _ansiConsole = ansiConsole;
        Out = new DuplexTextWriter(new ForwardingTextWriter(ansiConsole.Write!), additionalWriter);
    }

    public override Encoding OutputEncoding
    {
        get => _ansiConsole.Profile.Encoding;
        set => _ansiConsole.Profile.Encoding = value;
    }

    public override TextWriter Out { get; }

    public override int WindowWidth
    {
        get => _ansiConsole.Profile.Width;
        set => _ansiConsole.Profile.Width = value;
    }

    public override int WindowHeight
    {
        get => _ansiConsole.Profile.Height;
        set => _ansiConsole.Profile.Height = value;
    }

    public override ConsoleKeyInfo? ReadKey(bool intercept = false)
    {
        return _ansiConsole.Input.ReadKeyAsync(intercept, CancellationToken.None).Result;
    }
}