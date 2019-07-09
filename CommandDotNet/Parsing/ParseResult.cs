using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CommandDotNet.Parsing
{
    public class ParseResult
    {
        public ICommand Command { get; }
        public ReadOnlyCollection<ArgumentValues> Values { get; set; }
        public IReadOnlyCollection<string> RemainingArguments { get; }

        public ParseResult(ICommand command,
            IReadOnlyCollection<string> remainingArguments, 
            ReadOnlyCollection<ArgumentValues> values)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));
            RemainingArguments = remainingArguments ?? new List<string>().AsReadOnly();
            Values = values;
        }
    }
}