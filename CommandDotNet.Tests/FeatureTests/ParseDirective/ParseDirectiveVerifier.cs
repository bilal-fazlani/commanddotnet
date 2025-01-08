using System;
using System.Collections.Generic;
using CommandDotNet.Execution;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ParseDirective
{
    public class ParseDirectiveVerifier
    {
        public ParseDirectiveVerifier(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        public void Verify<TApp>(
            string? args, 
            bool includeTransforms = false,
            bool showsHelp = false,
            bool showsTokens = false,
            bool showsUnableToMapTokensToArgsMsg = false,
            bool showsHelpRequestOnlyTokensAvailableMsg = false,
            bool showsHintToUseParseT = false,
            bool throwBeforeBind = false,
            bool throwAtInvoke = false,
            bool exitBeforeBind = false,
            string? expectedParseResult = null,
            string? expectedResult = null) where TApp : class
        {
            var parse = includeTransforms ? "[parse:t]" : "[parse]";

            var contains = new List<string>();
            var notContains = new List<string>();

            void ContainsIf(bool yes, string value)
            {
                (yes ? contains! : notContains!).Add(value);
            }

            args ??= "";

            var helpRequested = args == "-h" || args.Contains(" -h") || args.Contains("-h ");
            
            ContainsIf(showsHelp, "Usage: testhost.dll ");
            ContainsIf(helpRequested, "Help requested. Only token transformations are available.");
            ContainsIf(showsTokens, @"token transformations:

>>> from shell");

            if (expectedParseResult != null)
            {
                contains.Add(expectedParseResult);
            }

            ContainsIf (showsUnableToMapTokensToArgsMsg,
                "Unable to map tokens to arguments. Falling back to token transformations.");
            
            ContainsIf(showsHelpRequestOnlyTokensAvailableMsg,
                "Help requested. Only token transformations are available.");

            ContainsIf(showsHintToUseParseT,
                "Use [parse:T] to include token transformations");

            var appRunner = new AppRunner<TApp>();

            if (exitBeforeBind)
            {
                appRunner.Configure(c => 
                    c.UseMiddleware((ctx, next) => ExitCodes.SuccessAsync, 
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
                        MiddlewareStages.Invoke, short.MinValue));
            }

            appRunner.Configure(c => 
                c.UseMiddleware((ctx, next) => 
                        throw new Exception("parse should exit before method invocation"),
                MiddlewareStages.Invoke, short.MaxValue-1));

            var result = appRunner
                .UseParseDirective()
                .UseErrorHandler((ctx, ex) => ExitCodes.ErrorAsync.Result)
                .Verify(new Scenario
                {
                    When = {Args = $"{parse} {args}"},
                    Then =
                    {
                        ExitCode = throwBeforeBind || (showsHelp && !helpRequested) ? 1 : 0,
                        Output = expectedResult,
                        OutputContainsTexts = contains,
                        OutputNotContainsTexts = notContains
                    }
                });

            if (showsHelp && showsTokens)
            {
                // help before token transformations
                var consoleOut = result.Console.OutText()!;
                var helpIndex = consoleOut.IndexOf("Usage: testhost.dll ", StringComparison.OrdinalIgnoreCase);
                var tokensIndex = consoleOut.IndexOf("token transformations:", StringComparison.OrdinalIgnoreCase);
                tokensIndex.Should().BeLessThan(helpIndex);
            }
        }
    }
}
