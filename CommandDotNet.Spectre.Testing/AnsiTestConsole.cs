using System;
using System.Collections.Generic;
using CommandDotNet.Rendering;
using CommandDotNet.TestTools;
using Spectre.Console;
using Spectre.Console.Rendering;
using Spectre.Console.Testing;
using SpectreTestConsole= Spectre.Console.Testing.TestConsole;

namespace CommandDotNet.Spectre.Testing
{
    public class AnsiTestConsole : ITestConsole, IAnsiConsole
    {
        private readonly SpectreTestConsole _spectreTestConsole;
        private readonly AnsiConsoleForwardingConsole _forwardingConsole;

        public AnsiTestConsole()
        {
            _spectreTestConsole = new SpectreTestConsole();
            _forwardingConsole = new AnsiConsoleForwardingConsole(_spectreTestConsole);
        }

        #region Implementation of ITestConsole

        // the SpectreTestConsole converts line endings to \n which can break assertions against literal strings in tests.
        // convert them back to Environment.NewLine

        public string AllText() => _spectreTestConsole.Output.Replace("\n", Environment.NewLine);

        public string OutText() => _spectreTestConsole.Output.Replace("\n", Environment.NewLine);

        public string ErrorText() => _spectreTestConsole.Output.Replace("\n", Environment.NewLine);

        public ITestConsole Mock(IEnumerable<string> pipedInput, bool overwrite = false)
        {
            throw new NotImplementedException();
        }

        public ITestConsole Mock(Func<ITestConsole, string?> onReadLine, bool overwrite = false)
        {
            throw new NotImplementedException();
        }

        public ITestConsole Mock(Func<ITestConsole, ConsoleKeyInfo>? onReadKey, bool overwrite = false)
        {
            throw new NotImplementedException();
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

        public bool IsOutputRedirected => _forwardingConsole.IsOutputRedirected;

        public IStandardStreamWriter Error => _forwardingConsole.Error;

        public bool IsErrorRedirected => _forwardingConsole.IsErrorRedirected;

        public IStandardStreamReader In => _forwardingConsole.In;

        public bool IsInputRedirected => _forwardingConsole.IsInputRedirected;

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