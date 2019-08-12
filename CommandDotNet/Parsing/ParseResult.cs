using System;
using System.Collections.Generic;
using CommandDotNet.Tokens;

namespace CommandDotNet.Parsing
{
    public class ParseResult
    {
        /// <summary>The command that addressed by the command line arguments</summary>
        public Command TargetCommand { get; }

        /// <summary>
        /// If extra operands were provided and <see cref="AppSettings.IgnoreUnexpectedOperands"/> is true,
        /// The extra operands will be stored in the <see cref="RemainingOperands"/> collection.
        /// </summary>
        public IReadOnlyCollection<Token> RemainingOperands { get; }

        /// <summary>
        /// All arguments provided after the argument separator "--" will be stored
        /// in the <see cref="SeparatedArguments"/> collection.
        /// </summary>
        public IReadOnlyCollection<Token> SeparatedArguments { get; }

        /// <summary>
        /// String values for defined arguments are stored in <see cref="ArgumentValues"/>.
        /// These values have not been converted to a system type yet. 
        /// </summary>
        public ArgumentValues ArgumentValues { get; }

        /// <summary>
        /// An exception encountered while parsing the commands.
        /// Help should be printed for the <see cref="TargetCommand"/>
        /// </summary>
        public Exception ParseError { get; }

        public ParseResult(Command command,
            IReadOnlyCollection<Token> remainingOperands,
            IReadOnlyCollection<Token> separatedArguments,
            ArgumentValues argumentValues)
        {
            TargetCommand = command ?? throw new ArgumentNullException(nameof(command));
            RemainingOperands = remainingOperands ?? new List<Token>();
            SeparatedArguments = separatedArguments ?? new List<Token>();
            ArgumentValues = argumentValues;
        }

        public ParseResult(Command command, Exception exception)
        {
            TargetCommand = command ?? throw new ArgumentNullException(nameof(command));
            ParseError = exception ?? throw new ArgumentNullException(nameof(exception));
            RemainingOperands = new List<Token>();
            SeparatedArguments = new List<Token>();
            ArgumentValues = new ArgumentValues();
        }
    }
}