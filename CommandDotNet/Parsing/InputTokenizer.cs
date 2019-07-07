using System;
using System.Linq;

namespace CommandDotNet.Parsing
{
    internal class InputTokenizer
    {
        private readonly AppSettings _appSettings;
        private readonly ParserContext _parserContext;
            
        public InputTokenizer(AppSettings appSettings, ParserContext parserContext)
        {
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            _parserContext = parserContext ?? throw new ArgumentNullException(nameof(parserContext));
        }

        public void Tokenize(ExecutionResult executionResult)
        {
            executionResult.OriginalTokens = executionResult.OriginalArgs.Tokenize(includeDirectives: _appSettings.EnableDirectives);

            // TODO: apply directives here:

            executionResult.FinalTokens = ApplyInputTransformations(executionResult.OriginalTokens);
        }

        private TokenCollection ApplyInputTransformations(TokenCollection args)
        {
            if (_parserContext.ParseDirectiveEnabled)
            {
                ReportTransformation(null, args);
            }

            var transformations = _parserContext.InputTransformations.OrderBy(t => t.Order).AsEnumerable();

            // append ExpandClubbedFlags to the end.
            // it's a feature we want to ensure is applied to all arguments
            // to prevent cases later where short clubbed options aren't found
            transformations = transformations.Union(
                new[]
                {
                    new InputTransformation(
                        "Expand clubbed flags",
                        Int32.MaxValue,
                        Tokenizer.ExpandClubbedOptions),
                });

            foreach (var transformation in transformations)
            {
                try
                {
                    var tempArgs = transformation.Transformation(args);

                    if (_parserContext.ParseDirectiveEnabled)
                    {
                        if (args.Count == tempArgs.Count &&
                            Enumerable.Range(0, args.Count).All(i => args[i] == tempArgs[i]))
                        {
                            ReportTransformation(transformation.Name, null);
                        }
                        else
                        {
                            ReportTransformation(transformation.Name, tempArgs);
                        }
                    }

                    args = tempArgs;
                }
                catch (Exception e)
                {
                    throw new AppRunnerException($"transformation failure for: {transformation}", e);
                }
            }

            return args;
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