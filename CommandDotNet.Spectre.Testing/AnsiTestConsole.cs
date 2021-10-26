using System;
using System.Collections.Generic;
using CommandDotNet.Rendering;
using CommandDotNet.TestTools;
using Spectre.Console;
using Spectre.Console.Rendering;
using Spectre.Console.Testing;
using SpectreTestConsole = Spectre.Console.Testing.TestConsole;
using TestConsole = CommandDotNet.TestTools.TestConsole;

namespace CommandDotNet.Spectre.Testing
{
    public class AnsiTestConsole : ITestConsole, IAnsiConsole
    {
        private readonly SpectreTestConsole _spectreTestConsole;
        private readonly AnsiConsoleForwardingConsole _forwardingConsole;
        private readonly TestConsole _testConsole;

        public AnsiTestConsole()
        {
            _spectreTestConsole = new SpectreTestConsole();
            _forwardingConsole = new AnsiConsoleForwardingConsole(_spectreTestConsole);
            _testConsole = new TestConsole();
        }

        #region Implementation of ITestConsole

        // the SpectreTestConsole converts line endings to \n which can break assertions against literal strings in tests.
        // convert them back to Environment.NewLine

        public string AllText() => _spectreTestConsole.Output.Replace("\n", Environment.NewLine);

        public string OutText() => _spectreTestConsole.Output.Replace("\n", Environment.NewLine);

        public string ErrorText() => _spectreTestConsole.Output.Replace("\n", Environment.NewLine);

        public ITestConsole Mock(IEnumerable<string> pipedInput, bool overwrite = false)
        {
            _testConsole.Mock(pipedInput, overwrite);
            return this;
        }

        public ITestConsole Mock(Func<ITestConsole, string?> onReadLine, bool overwrite = false)
        {
            _testConsole.Mock(onReadLine, overwrite);
            return this;
        }

        public ITestConsole Mock(Func<ITestConsole, ConsoleKeyInfo>? onReadKey, bool overwrite = false)
        {
            throw new NotSupportedException(
                $"When using the Spectre {nameof(AnsiTestConsole)}, " +
                "follow the patterns they use for testing prompts in their tests here: " +
                "https://github.com/spectreconsole/spectre.console/tree/main/test/Spectre.Console.Tests/Unit/Prompts");
        }

        #endregion

        #region Spectre.TestConsole members

        public void Clear(bool home) => _spectreTestConsole.Clear();

        public void Write(IRenderable renderable) => _spectreTestConsole.Write(renderable);

        public Profile Profile => _spectreTestConsole.Profile;

        IAnsiConsoleInput IAnsiConsole.Input => Input;

        public IExclusivityMode ExclusivityMode => _spectreTestConsole.ExclusivityMode;

        public TestConsoleInput Input => _spectreTestConsole.Input;

        public RenderPipeline Pipeline => _spectreTestConsole.Pipeline;

        public IAnsiConsoleCursor Cursor => _spectreTestConsole.Cursor;

        public string Output => _spectreTestConsole.Output;

        public IReadOnlyList<string> Lines => _spectreTestConsole.Lines;

        public bool EmitAnsiSequences
        {
            get => _spectreTestConsole.EmitAnsiSequences;
            set => _spectreTestConsole.EmitAnsiSequences = value;
        }

        #endregion

        #region AnsiConsoleForwardingConsole members

        public IStandardStreamWriter Out => _forwardingConsole.Out;

        public bool IsOutputRedirected => _testConsole.IsOutputRedirected;

        public IStandardStreamWriter Error => _forwardingConsole.Error;

        public bool IsErrorRedirected => _testConsole.IsErrorRedirected;

        public IStandardStreamReader In => _testConsole.In;

        public bool IsInputRedirected => _testConsole.IsInputRedirected;

        public ConsoleKeyInfo ReadKey(bool intercept = false)
        {
            return _forwardingConsole.ReadKey(intercept);
        }

        public bool TreatControlCAsInput
        {
            get => _forwardingConsole.TreatControlCAsInput;
            set => _forwardingConsole.TreatControlCAsInput = value;
        }

        #endregion
    }
}