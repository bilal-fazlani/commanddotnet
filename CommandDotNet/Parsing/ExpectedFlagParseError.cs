using CommandDotNet.Tokens;

namespace CommandDotNet.Parsing
{
    public class ExpectedFlagParseError : IParseError
    {
        public string Message { get; }
        public Command Command { get; }
        public Token ClubbedToken { get; }
        public string ShortName { get; }
        public Option? Option { get; }

        public ExpectedFlagParseError(Command command, Token clubbedToken, 
            string shortName, Option? option, string message)
        {
            Message = message;
            Command = command;
            ClubbedToken = clubbedToken;
            ShortName = shortName;
            Option = option;
        }
    }
}