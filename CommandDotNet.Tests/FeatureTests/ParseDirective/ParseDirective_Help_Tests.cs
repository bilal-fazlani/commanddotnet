using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ParseDirective
{
    public class ParseDirective_Help_Tests
    {
        private readonly ParseDirectiveVerifier _verifier;

        public ParseDirective_Help_Tests(ITestOutputHelper output)
        {
            _verifier = new ParseDirectiveVerifier(output);
        }

        [Fact]
        public void Given_HelpRequested_Parse_ShowsHelpThenTokens()
        {
            _verifier.Verify<App>("-h",
                showsHelp: true, 
                showsTokens: true,
                showsHelpRequestOnlyTokensAvailableMsg: true);
        }

        [Fact]
        public void Given_HelpRequested_ParseT_ShowsHelpThenTokens()
        {
            _verifier.Verify<App>("-h",
                includeTransforms: true,
                showsHelp: true,
                showsTokens: true,
                showsHelpRequestOnlyTokensAvailableMsg: true);
        }

        [Fact]
        public void Given_NoArgs_Parse_ShowsHelpThenTokens()
        {
            _verifier.Verify<App>(null,
                showsHelp: true,
                showsTokens: true,
                showsUnableToMapTokensToArgsMsg: true);
        }

        [Fact]
        public void Given_NoArgs_ParseT_ShowsHelpThenTokens()
        {
            _verifier.Verify<App>(null,
                includeTransforms: true,
                showsHelp: true,
                showsTokens: true,
                showsUnableToMapTokensToArgsMsg: true);
        }

        [Fact]
        public void Given_ExitBeforeBindValues_Parse_ShowsTokensOnly()
        {
            _verifier.Verify<App>("Cmd",
                showsHelp: false,
                showsTokens: true,
                showsUnableToMapTokensToArgsMsg: true,
                exitBeforeBind: true);
        }

        [Fact]
        public void Given_ExitBeforeBindValues_ParseT_ShowsTokensOnly()
        {
            _verifier.Verify<App>("Cmd",
                includeTransforms: true,
                showsHelp: false,
                showsTokens: true,
                showsUnableToMapTokensToArgsMsg: true,
                exitBeforeBind: true);
        }

        [Fact]
        public void Given_ThrowsBeforeBindValues_Parse_ShowsTokensOnly()
        {
            _verifier.Verify<App>("Cmd",
                showsHelp: false,
                showsTokens: true,
                showsUnableToMapTokensToArgsMsg: true,
                throwBeforeBind: true);
        }

        [Fact]
        public void Given_ThrowsBeforeBindValues_ParseT_ShowsTokensOnly()
        {
            _verifier.Verify<App>("Cmd",
                includeTransforms: true,
                showsHelp: false,
                showsTokens: true,
                showsUnableToMapTokensToArgsMsg: true,
                throwBeforeBind: true);
        }

        [Fact]
        public void Given_ThrowsAtInvoke_Parse_ShowsParse()
        {
            _verifier.Verify<App>("Cmd",
                showsHelp: false,
                showsTokens: false,
                throwAtInvoke: true,
                expectedParseResult: "command: Cmd");
        }

        [Fact]
        public void Given_ThrowsAtInvoke_ParseT_ShowsParse()
        {
            _verifier.Verify<App>("Cmd",
                includeTransforms: true,
                showsHelp: false,
                showsTokens: true,
                throwAtInvoke: true,
                expectedParseResult: "command: Cmd");
        }

        private class App
        {
            public void Cmd() { }
        }
    }
}