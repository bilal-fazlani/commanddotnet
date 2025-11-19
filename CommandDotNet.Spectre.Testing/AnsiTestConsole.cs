using System;
using System.Collections.Generic;
using System.IO;
using CommandDotNet.TestTools;
using Spectre.Console;
using Spectre.Console.Rendering;
using Spectre.Console.Testing;
using SpectreTestConsole = Spectre.Console.Testing.TestConsole;
using TestConsole = CommandDotNet.TestTools.TestConsole;

namespace CommandDotNet.Spectre.Testing;

/// <summary>
/// A test console that combines CommandDotNet's <see cref="TestConsole"/> with Spectre.Console's IAnsiConsole
/// for testing applications that use Spectre.Console features.
/// </summary>
/// <remarks>
/// <para><strong>Output Capture Architecture:</strong></para>
/// <list type="bullet">
/// <item>
/// <description><strong>Console.Out (stdout):</strong> Captured by Spectre's TestConsole.Output via DuplexTextWriter.
/// Accessible via <see cref="OutText()"/> or <see cref="AllText()"/>.</description>
/// </item>
/// <item>
/// <description><strong>Console.Error (stderr):</strong> Captured by base TestConsole.Error (Spectre's TestConsole 
/// doesn't support stderr capture - see https://github.com/spectreconsole/spectre.console/issues/1732).
/// Accessible via <see cref="ErrorText()"/> or <see cref="AllText()"/>.</description>
/// </item>
/// <item>
/// <description><strong>IAnsiConsole API calls:</strong> Rendered through Spectre's TestConsole and captured 
/// in TestConsole.Output.</description>
/// </item>
/// </list>
/// <para><strong>Usage:</strong> Use <see cref="AllText()"/> for combined stdout+stderr output (as it would appear 
/// in a terminal), or use <see cref="OutText()"/> and <see cref="ErrorText()"/> to assert on streams separately.</para>
/// </remarks>
public class AnsiTestConsole : TestConsole, IAnsiConsole
{
    internal SpectreTestConsole SpectreTestConsole { get; }
    private readonly AnsiConsoleForwardingConsole _forwardingConsole;

    public AnsiTestConsole(bool trimEnd = true) : base(trimEnd)
    {
        SpectreTestConsole = new SpectreTestConsole();

        // capture Spectre output to TestConsoleWriter
        _forwardingConsole = new AnsiConsoleForwardingConsole(SpectreTestConsole, base.Out);
    }

    #region Implementation of ITestConsole

    // NOTE: Spectre's TestConsole only captures stdout (Console.Out), NOT stderr (Console.Error)
    // See: https://github.com/spectreconsole/spectre.console/issues/1732
    // Therefore:
    // - Console.Out writes go through DuplexTextWriter to BOTH SpectreTestConsole.Output AND base.Out
    // - Console.Error writes only go to base.Error (not captured by Spectre)
    //
    // The SpectreTestConsole converts line endings to \n which can break assertions against literal strings in tests.
    // We convert them back to Environment.NewLine for consistency.

    public override string AllText()
    {
        // Returns combined stdout + stderr. The error text will appear after output, even when error text was written first
        // - stdout: from SpectreTestConsole.Output (captures Console.Out via DuplexTextWriter)
        // - stderr: from base.ErrorText() (Console.Error not supported by Spectre's TestConsole)
        // Note: We use base.ErrorText() instead of base.AllText() to avoid Console.Out duplication
        var spectre = SpectreTestConsole.Output.Replace("\n", Environment.NewLine);
        var error = base.ErrorText();
        return string.IsNullOrEmpty(error)
            ? spectre 
            : spectre + error;
    }

    public override string OutText()
    {
        // Returns only stdout (Console.Out)
        // All Console.Out writes go through DuplexTextWriter to both Spectre and base.Out,
        // so we only return SpectreTestConsole.Output to avoid duplication.
        return SpectreTestConsole.Output.Replace("\n", Environment.NewLine);
    }
        
    public override ITestConsole Mock(Func<ITestConsole, ConsoleKeyInfo>? onReadKey, bool overwrite = false)
    {
        throw new NotSupportedException(
            $"When using the Spectre {nameof(AnsiTestConsole)}, " +
            "follow the patterns they use for testing prompts in their tests here: " +
            "https://github.com/spectreconsole/spectre.console/tree/main/test/Spectre.Console.Tests/Unit/Prompts");
    }

    #endregion

    #region Spectre.TestConsole members

    public void Clear(bool home) => SpectreTestConsole.Clear();

    public void Write(IRenderable renderable) => SpectreTestConsole.Write(renderable);

    public Profile Profile => SpectreTestConsole.Profile;

    IAnsiConsoleInput IAnsiConsole.Input => Input;

    public IExclusivityMode ExclusivityMode => SpectreTestConsole.ExclusivityMode;

    public TestConsoleInput Input => SpectreTestConsole.Input;

    public RenderPipeline Pipeline => SpectreTestConsole.Pipeline;

    public IAnsiConsoleCursor Cursor => SpectreTestConsole.Cursor;

    public string Output => SpectreTestConsole.Output;

    public IReadOnlyList<string> Lines => SpectreTestConsole.Lines;

    public bool EmitAnsiSequences
    {
        get => SpectreTestConsole.EmitAnsiSequences;
        set => SpectreTestConsole.EmitAnsiSequences = value;
    }

    #endregion

    #region AnsiConsoleForwardingConsole members

    public override TextWriter Out => _forwardingConsole.Out;

    public override ConsoleKeyInfo? ReadKey(bool intercept = false)
    {
        return _forwardingConsole.ReadKey(intercept);
    }

    public override bool TreatControlCAsInput
    {
        get => _forwardingConsole.TreatControlCAsInput;
        set => _forwardingConsole.TreatControlCAsInput = value;
    }

    #endregion
}