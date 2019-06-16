using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Extensions;

namespace CommandDotNet.Parsing
{
    internal class ParserBuilder
    {
        internal Dictionary<string, ArgumentTransformation> ArgumentTransformationsByName
            = new Dictionary<string, ArgumentTransformation>();

        public void AddArgumentTransformation(string name, int order, Func<Tokens,Tokens> transformation)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (transformation == null) throw new ArgumentNullException(nameof(transformation));

            if (ArgumentTransformationsByName.ContainsKey(name))
            {
                throw new ArgumentException(
                    $"a transformation named {name} already exists. " +
                    $"transformations={ArgumentTransformationsByName.Keys.ToOrderedCsv()}");
            }
            ArgumentTransformationsByName.Add(name, new ArgumentTransformation(name, order, transformation));
        }

        public ParserContext Build()
        {
            return new ParserContext
            {
                ArgumentTransformations = ArgumentTransformationsByName.Values.OrderBy(t => t.Order)
            };
        }
    }
}