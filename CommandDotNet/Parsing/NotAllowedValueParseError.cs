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
            Message = $"Unrecognized value '{token.RawValue}' for {(argument is Option ? "option" : "argument")}: {argument.Name}";
        }
    }
}