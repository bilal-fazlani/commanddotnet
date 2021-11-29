using System;
using System.Collections.Generic;
using System.IO;
using CommandDotNet.TestTools;
using Spectre.Console;
using Spectre.Console.Rendering;
using Spectre.Console.Testing;
using SpectreTestConsole = Spectre.Console.Testing.TestConsole;
using TestConsole = CommandDotNet.TestTools.TestConsole;

namespace CommandDotNet.Spectre.Testing
{
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

        // the SpectreTestConsole converts line endings to \n which can break assertions against literal strings in tests.
        // convert them back to Environment.NewLine

        public override string? AllText()
        {
            var spectre = SpectreTestConsole.Output.Replace("\n", Environment.NewLine);
            var nonSpectre = base.AllText();
            return nonSpectre is null 
                ? spectre 
                : spectre + nonSpectre;
        }

        public override string? OutText()
        {
            var spectre = SpectreTestConsole.Output.Replace("\n", Environment.NewLine);
            var nonSpectre = base.OutText();
            return nonSpectre is null
                ? spectre
                : spectre + nonSpectre;
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
}