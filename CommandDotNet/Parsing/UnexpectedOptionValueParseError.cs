using System;
using CommandDotNet.Tokens;

namespace CommandDotNet.Parsing
{
    /// <summary>
    /// A value was provided for an option that didn't expect it.<br/>
    /// The option has an arity of 1 but was given multiple values<br/>
    /// Or the option has an arity of 0 but was given a value.
    /// </summary>
    public class UnexpectedOptionValueParseError : IParseError
    {
        public string Message { get; }
        public Command Command { get; }
        public Option Option { get; }
        public Token Token { get; }

        public UnexpectedOptionValueParseError(Command command, Option option, Token token)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));
            Option = option ?? throw new ArgumentNullException(nameof(option));
            Token = token ?? throw new ArgumentNullException(nameof(token));
            Message = Resources.A.Parse_Unexpected_value_for_option(token.RawValue, option.Name);
        }
    }
}