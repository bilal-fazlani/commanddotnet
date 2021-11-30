using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ParseDirective
{
    public class ParseDirective_CommandsAndSubCommands_Tests
    {
        private readonly ParseDirectiveVerifier _verifier;

        public ParseDirective_CommandsAndSubCommands_Tests(ITestOutputHelper output)
        {
            _verifier = new ParseDirectiveVerifier(output);
        }

        [Fact]
        public void When_NoArgs_Given_NoDefaultCommands_Then_Parse_ShowsHelpThenTokens()
        {
            _verifier.Verify<NoDefaultCommands>(null,
                showsTokens: true,
                showsHelp: true,
                showsUnableToMapTokensToArgsMsg: true);
        }

        [Fact]
        public void When_NoArgs_Given_NoDefaultCommands_Then_ParseT_ShowsHelpThenTokens()
        {
            _verifier.Verify<NoDefaultCommands>(null,
                includeTransforms: true,
                showsTokens: true,
                showsHelp: true,
                showsUnableToMapTokensToArgsMsg: true);
        }

        [Fact]
        public void When_NoArgs_Given_WithDefaultCommands_Then_Parse_ShowsParseOnly()
        {
            _verifier.Verify<WithDefaultCommands>(null,
                expectedParseResult:"command: (root)");
        }

        [Fact]
        public void When_NoArgs_Given_WithDefaultCommands_Then_ParseT_ShowsParseThenTokens()
        {
            _verifier.Verify<WithDefaultCommands>(null,
                includeTransforms: true,
                showsTokens: true,
                expectedParseResult: "command: (root)");
        }

        [Fact]
        public void When_ExecCommand_Given_NoDefaultCommands_Then_Parse_ShowsParseOnly()
        {
            _verifier.Verify<NoDefaultCommands>("Cmd",
                expectedParseResult: "command: Cmd");
        }

        [Fact]
        public void When_ExecCommand_Given_NoDefaultCommands_Then_ParseT_ShowsParseThenTokens()
        {
            _verifier.Verify<NoDefaultCommands>("Cmd",
                includeTransforms: true,
                showsTokens: true,
                expectedParseResult: "command: Cmd");
        }

        [Fact]
        public void When_ExecCommand_Given_WithDefaultCommands_Then_Parse_ShowsParseOnly()
        {
            _verifier.Verify<WithDefaultCommands>("Cmd",
                expectedParseResult: "command: Cmd");
        }

        [Fact]
        public void When_ExecCommand_Given_WithDefaultCommands_Then_ParseT_ShowsParseThenTokens()
        {
            _verifier.Verify<WithDefaultCommands>("Cmd",
                includeTransforms: true,
                showsTokens: true,
                expectedParseResult: "command: Cmd");
        }

        [Fact]
        public void When_ExecNestedCommand_Given_NoDefaultCommands_Then_Parse_ShowsHelpThenTokens()
        {
            _verifier.Verify<NoDefaultCommands>("Nested",
                showsTokens: true,
                showsHelp: true,
                showsUnableToMapTokensToArgsMsg: true);
        }

        [Fact]
        public void When_ExecNestedCommand_Given_NoDefaultCommands_Then_ParseT_ShowsHelpThenTokens()
        {
            _verifier.Verify<NoDefaultCommands>("Nested",
                includeTransforms: true,
                showsTokens: true,
                showsHelp: true,
                showsUnableToMapTokensToArgsMsg: true);
        }

        [Fact]
        public void When_ExecNestedCommand_Given_WithDefaultCommands_Then_Parse_ShowsParseOnly()
        {
            _verifier.Verify<WithDefaultCommands>("Nested",
                expectedParseResult: "command: Nested");
        }

        [Fact]
        public void When_ExecNestedCommand_Given_WithDefaultCommands_Then_ParseT_ShowsParseThenTokens()
        {
            _verifier.Verify<WithDefaultCommands>("Nested",
                includeTransforms: true,
                showsTokens: true,
                expectedParseResult: "command: Nested");
        }

        [Fact]
        public void When_ExecSubcommandCommand_Given_NoDefaultCommands_Then_Parse_ShowsParseOnly()
        {
            _verifier.Verify<NoDefaultCommands>("Nested Cmd",
                expectedParseResult: "command: Nested Cmd");
        }

        [Fact]
        public void When_ExecSubcommandCommand_Given_NoDefaultCommands_Then_ParseT_ShowsParseThenTokens()
        {
            _verifier.Verify<NoDefaultCommands>("Nested Cmd",
                includeTransforms: true,
                showsTokens: true,
                expectedParseResult: "command: Nested Cmd");
        }

        [Fact]
        public void When_ExecSubcommandCommand_Given_WithDefaultCommands_Then_Parse_ShowsParseOnly()
        {
            _verifier.Verify<WithDefaultCommands>("Nested Cmd",
                expectedParseResult: "command: Nested Cmd");
        }

        [Fact]
        public void When_ExecSubcommandCommand_Given_WithDefaultCommands_Then_ParseT_ShowsParseThenTokens()
        {
            _verifier.Verify<WithDefaultCommands>("Nested Cmd",
                includeTransforms: true,
                showsTokens: true,
                expectedParseResult: "command: Nested Cmd");
        }

        public class NoDefaultCommands
        {
            public void Cmd() { }

            [Subcommand]
            public class Nested
            {
                public void Cmd() { }
            }
        }

        public class WithDefaultCommands
        {
            public void Cmd() { }

            [DefaultCommand]
            public void DefaultCmd() { }

            [Subcommand]
            public class Nested
            {
                [DefaultCommand]
                public void DefaultCmd() { }
                public void Cmd() { }
            }
        }
    }
}