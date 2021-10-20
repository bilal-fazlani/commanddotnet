using System;
using CommandDotNet.Tokens;

namespace CommandDotNet.Parsing
{
    /// <summary>The value is not in <see cref="IArgument.AllowedValues"/></summary>
    public class NotAllowedValueParseError : IParseError
    {
        public string Message { get; }
        public Command Command { get; }
        public IArgument Argument { get; }
        public Token Token { get; }

        public NotAllowedValueParseError(Command command, IArgument argument, Token token)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));
            Argument = argument ?? throw new ArgumentNullException(nameof(argument));
            Token = token ?? throw new ArgumentNullException(nameof(token));
            Message = Resources.A.Parse_Unrecognized_value_for(token.RawValue,
                (argument is Option ? Resources.A.Common_option_lc : Resources.A.Common_argument_lc),
                argument.Name);
        }
    }
}