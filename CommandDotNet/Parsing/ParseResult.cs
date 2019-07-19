using System;
using System.Collections.Generic;

namespace CommandDotNet.Parsing
{
    public class ParseResult
    {
        public ICommand TargetCommand { get; }
        public IReadOnlyCollection<Token> RemainingArguments { get; }
        public IReadOnlyCollection<Token> SeparatedArguments { get; }
        public ArgumentValues ArgumentValues { get; }

        public ParseResult(ICommand command,
            IReadOnlyCollection<Token> remainingArguments,
            IReadOnlyCollection<Token> separatedArguments,
            ArgumentValues argumentValues)
        {
            TargetCommand = command ?? throw new ArgumentNullException(nameof(command));
            RemainingArguments = remainingArguments ?? new List<Token>();
            SeparatedArguments = separatedArguments ?? new List<Token>();
            ArgumentValues = argumentValues;
        }
    }
}