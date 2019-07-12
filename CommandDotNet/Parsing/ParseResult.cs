using System;
using System.Collections.Generic;

namespace CommandDotNet.Parsing
{
    public class ParseResult
    {
        public ICommand Command { get; }
        public IReadOnlyCollection<Token> RemainingArguments { get; }
        public IReadOnlyCollection<Token> SeparatedArguments { get; }
        public Dictionary<IArgument, List<string>> ValuesByArgument { get; }

        public ParseResult(ICommand command,
            IReadOnlyCollection<Token> remainingArguments,
            IReadOnlyCollection<Token> separatedArguments,
            Dictionary<IArgument, List<string>> valuesByArgument)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));
            RemainingArguments = remainingArguments ?? new List<Token>();
            SeparatedArguments = separatedArguments ?? new List<Token>();
            ValuesByArgument = valuesByArgument;
        }
    }
}