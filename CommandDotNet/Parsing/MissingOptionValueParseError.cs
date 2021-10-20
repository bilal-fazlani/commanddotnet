using System;

namespace CommandDotNet.Parsing
{
    /// <summary>An option was specified without a value</summary>
    public class MissingOptionValueParseError : IParseError
    {
        public string Message { get; }
        public Command Command { get; }
        public Option Option { get; }

        public MissingOptionValueParseError(Command command, Option option)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));
            Option = option ?? throw new ArgumentNullException(nameof(option));
            Message = Resources.A.Parse_Missing_value_for_option(option.Name);
        }
    }
}