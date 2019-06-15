using System;

namespace CommandDotNet.Parsing
{
    public class ArgumentTransformation
    {
        public static class Orders
        {
            public const int UnclubFlags = 100;
        }

        public string Name { get; }
        public int Order { get; }
        public Func<string[], string[]> Transformation { get; }

        public ArgumentTransformation(string name, int order, Func<string[],string[]> transformation)
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