using System;

namespace CommandDotNet.Tokens
{
    public class TokenTransformation
    {
        public string Name { get; }
        public int Order { get; }
        public Func<CommandContext, TokenCollection, TokenCollection> Transformation { get; }

        public TokenTransformation(string name, int order, Func<CommandContext, TokenCollection, TokenCollection> transformation)
        {
            Name = name;
            Order = order;
            Transformation = transformation;
        }

        public override string ToString()
        {
            return $"{nameof(TokenTransformation)}: {Name} ({Order})";
        }
    }
}