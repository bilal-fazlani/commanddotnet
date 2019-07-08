using CommandDotNet.Parsing;

namespace CommandDotNet
{
    public class OriginalInput
    {
        public string[] Args { get; }
        public TokenCollection Tokens { get; }

        public OriginalInput(string[] args, TokenCollection tokens)
        {
            Args = args;
            Tokens = tokens;
        }
    }
}