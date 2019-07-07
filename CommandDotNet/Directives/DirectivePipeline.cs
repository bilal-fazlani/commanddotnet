using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Extensions;
using CommandDotNet.Parsing;

namespace CommandDotNet.Directives
{
    internal class DirectivePipeline
    {
        private readonly AppSettings _appSettings;
        private readonly ParserContext _parserContext;

        public DirectivePipeline(AppSettings appSettings, ParserContext parserContext)
        {
            _appSettings = appSettings;
            _parserContext = parserContext;
        }

        // this is obviously not the ideal design.  major code smell.
        // but... it meets our needs simply until we have settled
        // on a better design for implementing the control flow
        // i.e. middleware pipeline
        internal static bool InTestHarness { private get; set; }

        public void ProcessDirectives(ExecutionResult executionResult)
        {
            if (!_appSettings.EnableDirectives)
            {
                return;
            }

            // adapted from https://github.com/dotnet/command-line-api directives

            if (executionResult.Tokens.TryGetDirective("debug", out string value))
            {
                var process = Process.GetCurrentProcess();

                var processId = process.Id;

                _appSettings.Out.WriteLine($"Attach your debugger to process {processId} ({process.ProcessName}).");

                while (!InTestHarness && !Debugger.IsAttached)
                {
                    Task.Delay(500);
                }
            }
            
            if (executionResult.Tokens.TryGetDirective("parse", out value))
            {
                var parts = value.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                var verbose = parts.Length > 1 && parts[1].Equals("verbose", StringComparison.OrdinalIgnoreCase);

                if (!verbose)
                {
                    _appSettings.Out.WriteLine("use [parse:verbose] to see results after each transformation");
                }

                ReportTransformation(null, executionResult.Tokens);

                if (verbose)
                {
                    _parserContext.OnInputTransformation += tuple =>
                    {
                        if (tuple.pre.Count == tuple.post.Count &&
                            Enumerable.Range(0, tuple.pre.Count).All(i => tuple.pre[i] == tuple.post[i]))
                        {
                            ReportTransformation(tuple.transformation.Name, null);
                        }
                        else
                        {
                            ReportTransformation(tuple.transformation.Name, tuple.post);
                        }
                    };
                }
                else
                {
                    _parserContext.OnTokenizationCompleted += result => 
                        ReportTransformation(_parserContext.InputTransformations.Select(t => t.Name).ToCsv(" > "), result.Tokens);
                }

                _parserContext.OnTokenizationCompleted += result => result.ShouldExitWithCode(0);
            }
        }


        private void ReportTransformation(string name, TokenCollection args)
        {
            var maxTokenTypeNameLength = Enum.GetNames(typeof(TokenType)).Max(n => n.Length);

            if (args == null)
            {
                _appSettings.Out.WriteLine($">>> no changes after: {name}");
            }
            else
            {
                _appSettings.Out.WriteLine(name == null ? ">>> from shell" : $">>> transformed after: {name}");
                foreach (var arg in args)
                {
                    var outputFormat = $"  {{0, -{maxTokenTypeNameLength}}}: {{1}}";
                    _appSettings.Out.WriteLine(outputFormat, arg.TokenType, arg.RawValue);
                }
            }
            _appSettings.Out.WriteLine();
        }
    }
}