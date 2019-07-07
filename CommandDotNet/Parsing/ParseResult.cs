using System;
using System.Collections.Generic;

namespace CommandDotNet.Parsing
{
    public class ParseResult
    {
        public ICommand Command { get; }
        public IReadOnlyCollection<string> RemainingArguments { get; }

        public ParseResult(
            ICommand command,
            IReadOnlyCollection<string> remainingArguments)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));
            RemainingArguments = remainingArguments ?? new List<string>().AsReadOnly();
        }
    }
}