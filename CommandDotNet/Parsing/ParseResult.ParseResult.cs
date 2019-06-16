using System.Linq;
using CommandDotNet.MicrosoftCommandLineUtils;

namespace CommandDotNet.Parsing
{
    public class ParseResult
    {
        public ICommand Command { get; }
        public string[] OriginalArgs { get; }
        public Tokens Tokens { get; }
        public int? ExitCode { get; }
        public Tokens UnparsedTokens { get; }

        public ParseResult(
            ICommand command,
            string[] originalArgs,
            Tokens tokens,
            int? exitCode = null,
            Tokens unparsedTokens = null)
        {
            Command = command;
            OriginalArgs = originalArgs;
            Tokens = tokens;
            ExitCode = exitCode;
            UnparsedTokens = unparsedTokens ?? new Tokens(Enumerable.Empty<Token>());
        }
    }
}