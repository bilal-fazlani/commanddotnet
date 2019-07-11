using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Extensions;
using CommandDotNet.Parsing;

namespace CommandDotNet.Execution
{
    internal class ExecutionBuilder
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

        public ExecutionConfig Build()
        {
            return new ExecutionConfig
            {
                MiddlewarePipeline = _middlewares.OrderBy(m => m.order).Select(m => m.middleware).ToArray(),
                InputTransformations = _inputTransformationsByName.Values.OrderBy(t => t.Order).ToArray()
            };
        }
    }
}