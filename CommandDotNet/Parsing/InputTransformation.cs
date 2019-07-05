using System;

namespace CommandDotNet.Parsing
{
    public class InputTransformation
    {
        public string Name { get; }
        public int Order { get; }
        public Func<TokenCollection, TokenCollection> Transformation { get; }

        public InputTransformation(string name, int order, Func<TokenCollection, TokenCollection> transformation)
        {
            Name = name;
            Order = order;
            Transformation = transformation;
        }

        public override string ToString()
        {
            return $"{nameof(InputTransformation)}: {Name} ({Order})";
        }
    }
}