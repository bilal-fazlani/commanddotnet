using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Extensions;

namespace CommandDotNet.Parsing
{
    internal class ParserBuilder
    {
        private readonly Dictionary<string, InputTransformation> _inputTransformationsByName
            = new Dictionary<string, InputTransformation>();

        public void AddInputTransformation(string name, int order, Func<TokenCollection,TokenCollection> transformation)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (transformation == null) throw new ArgumentNullException(nameof(transformation));

            if (_inputTransformationsByName.ContainsKey(name))
            {
                throw new ArgumentException(
                    $"a transformation named {name} already exists. " +
                    $"transformations={_inputTransformationsByName.Keys.ToOrderedCsv()}");
            }
            _inputTransformationsByName.Add(name, new InputTransformation(name, order, transformation));
        }

        public ParserContext Build(ExecutionResult executionResult)
        {
            return new ParserContext(executionResult)
            {
                InputTransformations = _inputTransformationsByName.Values.OrderBy(t => t.Order)
            };
        }
    }
}