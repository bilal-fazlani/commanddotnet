using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommandDotNet.Execution;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ParseDirective
{
    public class ParseDirectiveVerifier
    {
        private readonly ITestOutputHelper _output;

        public ParseDirectiveVerifier(ITestOutputHelper output)
        {
            _output = output;
        }

        public void Verify<TApp>(
            string args, 
            bool includeTransforms = false,
            bool showsHelp = false,
            bool showsTokens = false,
            bool showsUnableToMapTokensToArgsMsg = false,
            bool showsHelpRequestOnlyTokensAvailableMsg = false,
            bool showsHintToUseParseT = false,
            bool throwBeforeBind = false,
            bool throwAtInvoke = false,
            bool exitBeforeBind = false,
            string expectedParseResult = null,
            string expectedResult = null) where TApp : class
        {
            var parse = includeTransforms ? "[parse:t]" : "[parse]";

            var contains = new List<string>();
            var notContains = new List<string>();

            void containsIf(bool yes, string value)
            {
                (yes ? contains : notContains).Add(value);
            }

            args = args ?? "";

            containsIf(showsHelp, "Usage: dotnet testhost.dll ");
            containsIf(args == "-h" || args.Contains(" -h") || args.Contains("-h "),
                "Help requested. Only token transformations are available.");
            containsIf(showsTokens, @"token transformations:

>>> from shell");

            if (expectedParseResult != null)
            {
                contains.Add(expectedParseResult);
            }

            containsIf (showsUnableToMapTokensToArgsMsg,
                "Unable to map tokens to arguments. Falling back to token transformations.");
            
            containsIf(showsHelpRequestOnlyTokensAvailableMsg,
                "Help requested. Only token transformations are available.");

            containsIf(showsHintToUseParseT,
                "Use [parse:T] to include token transformations");

            var appRunner = new AppRunner<TApp>();

            if (exitBeforeBind)
            {
                appRunner.Configure(c => 
                    c.UseMiddleware((ctx, next) => Task.FromResult(0), 
                        MiddlewareStages.PostParseInputPreBindValues));
            }

            if (throwBeforeBind)
            {
                appRunner.Configure(c =>
                    c.UseMiddleware((ctx, next) => 
                            throw new Exception("throwBeforeBind exception"),
                        MiddlewareStages.PostParseInputPreBindValues));
            }

            if (throwAtInvoke)
            {
                appRunner.Configure(c =>
                    c.UseMiddleware((ctx, next) => 
                            throw new Exception("throwAtInvoke exception"),
                        MiddlewareStages.Invoke, int.MinValue));
            }

            appRunner.Configure(c => 
                c.UseMiddleware((ctx, next) => 
                        throw new Exception("parse should exit before method invocation"),
                MiddlewareStages.Invoke, int.MaxValue-1));

            var result = appRunner
                .UseParseDirective()
                .VerifyScenario(_output,
                    new Scenario
                    {
                        WhenArgs = $"{parse} {args}",
                        Then =
                        {
                            ExitCode = throwBeforeBind ? 1 : 0,
                            Result = expectedResult,
                            ResultsContainsTexts = contains,
                            ResultsNotContainsTexts = notContains
                        }
                    });

            if (showsHelp && showsTokens)
            {
                // help before token transformations
                var helpIndex = result.ConsoleOut.IndexOf("Usage: dotnet testhost.dll ");
                var tokensIndex = result.ConsoleOut.IndexOf("token transformations:");
                tokensIndex.Should().BeLessThan(helpIndex);
            }
        }
    }
}