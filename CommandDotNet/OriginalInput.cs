using System.Collections.Generic;
using CommandDotNet.Tokens;

namespace CommandDotNet
{
    public class OriginalInput
    {
        /// <summary>The original string array passed to the program</summary>
        public IReadOnlyCollection<string> Args { get; }

        /// <summary>The original tokens before any input transformations were applied</summary>
        public TokenCollection Tokens { get; }

        public OriginalInput(string[] args, TokenCollection tokens)
        {
            Args = args;
            Tokens = tokens;
        }
    }
}