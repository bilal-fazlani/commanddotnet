using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Extensions;

namespace CommandDotNet.Parsing
{
    // TODO: return Task
    public delegate int ExecutionMiddleware(
        ExecutionResult context,
        Func<ExecutionResult, int> next);

    public enum MiddlewareStages
    {
        Configuration = 100,
        Directives = Configuration + 100,
        Tokenize = Directives + 100,
        Parsing = Tokenize + 100,
        Invocation = Parsing + 100,
    }

    internal class ParserBuilder
    {
        private readonly List<(ExecutionMiddleware middleware, int order)> _middlewares = new List<(ExecutionMiddleware middleware, int order)>();

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

        public void AddMiddlewareInStage(ExecutionMiddleware middleware, MiddlewareStages stage, int orderWithinStage = 0)
        {
            AddMiddleware(middleware, (int)stage + orderWithinStage);
        }

        public void AddMiddleware(ExecutionMiddleware middleware, int order)
        {
            _middlewares.Add((middleware, order));
        }

        public ParserConfig Build(ExecutionResult executionResult)
        {
            return new ParserConfig(executionResult)
            {
                MiddlewarePipeline = _middlewares.OrderBy(m => m.order).Select(m => m.middleware).ToArray(),
                InputTransformations = _inputTransformationsByName.Values.OrderBy(t => t.Order).ToArray()
            };
        }
    }
}