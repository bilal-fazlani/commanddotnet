using System;
using System.Diagnostics;
using System.Text;
using CommandDotNet.Diagnostics;
using static System.Environment;

namespace CommandDotNet
{
    /// <summary>
    /// Note: Our cheap and cheerful CliWrap.<br/>
    /// This class exists to avoid external dependencies for core features.<br/>
    /// Checkout CliWrap if you need a more features than are exposed here.
    /// </summary>
    public class ExternalCommand
    {
        private readonly StringBuilder _stdOut = new();
        private readonly StringBuilder _stdErr = new();
        
        private readonly IConsole? _console;

        public string FileName { get; }
        public string Arguments { get; }
        public int? ExitCode { get; private set; }
        public Exception? Error { get; private set; }

        public bool Succeeded => Error is null && ExitCode is 0;

        public TimeSpan Elapsed { get; set; }
        
        public string StandardOutput => _stdOut.ToString();
        public string StandardError => _stdErr.ToString();

        public static ExternalCommand Run(string fileName, string arguments, IConsole? console = null) => 
            new ExternalCommand(fileName, arguments, console).Run();

        private ExternalCommand(string fileName, string arguments, IConsole? console = null)
        {
            FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
            Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
            _console = console;
        }
        
        private ExternalCommand Run()
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = FileName,
                    Arguments = Arguments,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                }
            };
            
            process.OutputDataReceived += (sender, args) => _stdOut.Append(args.Data);
            process.ErrorDataReceived += (sender, args) => _stdErr.Append(args.Data);
            if (_console != null)
            {
                process.OutputDataReceived += (sender, args) => _console.Out.Write(args.Data);
                process.ErrorDataReceived += (sender, args) => _console.Error.Write(args.Data);
            }
            
            try
            {
                var sw = Stopwatch.StartNew();
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
                ExitCode = process.ExitCode;
                Elapsed = sw.Elapsed;
            }
            catch (Exception e)
            {
                Error = e;
            }
            
            return this;
        }

        public string ToString(Indent indent) => ToString(indent, false);

        public string ToString(Indent indent, bool skipOutputs)
        {
            indent = indent.Increment();
            var ouptut = $"{nameof(ExternalCommand)}:{NewLine}" +
                         $"{indent}Command: {FileName}{NewLine}" +
                         $"{indent}Arguments: {Arguments}{NewLine}" +
                         $"{indent}ExitCode: {ExitCode}{NewLine}" +
                         $"{indent}Succeeded: {Succeeded}{NewLine}" +
                         $"{indent}Elapsed: {Elapsed}{NewLine}" +
                         $"{indent}Error: {Error?.Print(indent, includeProperties: true, includeData: true)}{NewLine}";
            return skipOutputs
                ? ouptut
                : ouptut +
                  $"{indent}Standard Output: {StandardOutput}{NewLine}" +
                  $"{indent}Error Output: {StandardError}{NewLine}";
        }
    }
}