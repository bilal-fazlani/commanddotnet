using System;

namespace CommandDotNet.Parsing
{
    public class ArgumentTransformation
    {
        public string Name { get; }
        public int Order { get; }
        public Func<Tokens, Tokens> Transformation { get; }

        public ArgumentTransformation(string name, int order, Func<Tokens, Tokens> transformation)
        {
            Name = name;
            Order = order;
            Transformation = transformation;
        }

        public override string ToString()
        {
            return $"{nameof(ArgumentTransformation)}: {Name} ({Order})";
        }
    }
}