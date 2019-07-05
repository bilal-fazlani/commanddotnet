using System.Linq;

namespace CommandDotNet.Parsing
{
    public class ParseResult
    {
        public ICommand Command { get; }
        public string[] OriginalArgs { get; }
        public TokenCollection TokenCollection { get; }
        public int? ExitCode { get; }
        public TokenCollection UnparsedTokenCollection { get; }
        public IContextData ContextData { get; } = new ContextData();

        public ParseResult(
            ICommand command,
            string[] originalArgs,
            TokenCollection tokenCollection,
            int? exitCode = null,
            TokenCollection unparsedTokenCollection = null)
        {
            Command = command;
            OriginalArgs = originalArgs;
            TokenCollection = tokenCollection;
            ExitCode = exitCode;
            UnparsedTokenCollection = unparsedTokenCollection ?? new TokenCollection(Enumerable.Empty<Token>());
        }
    }
}