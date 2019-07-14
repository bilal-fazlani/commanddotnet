using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Builders;
using CommandDotNet.Extensions;
using CommandDotNet.Parsing;

namespace CommandDotNet.Execution
{
    internal class ExecutionBuilder
    {
        private readonly SortedDictionary<MiddlewareStages, List<(ExecutionMiddleware middleware, int order)>> _middlewaresByStage = 
            new SortedDictionary<MiddlewareStages, List<(ExecutionMiddleware middleware, int order)>>();

        private readonly Dictionary<string, InputTransformation> _inputTransformationsByName
            = new Dictionary<string, InputTransformation>();
        public ParseEvents ParseEvents { get; } = new ParseEvents();
        public BuildEvents BuildEvents { get; } = new BuildEvents();

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
            _middlewaresByStage
                .GetOrAdd(stage, s => new List<(ExecutionMiddleware middleware, int order)>())
                .Add((middleware, orderWithinStage));
        }

        public ExecutionConfig Build(AppSettings appSettings, IDependencyResolver dependencyResolver)
        {
            return new ExecutionConfig(appSettings, dependencyResolver, ParseEvents, BuildEvents)
            {
                MiddlewarePipeline = _middlewaresByStage
                    .SelectMany(kvp => kvp.Value.Select(v => new {stage = kvp.Key, v.order, v.middleware}) )
                    .OrderBy(m => m.stage)
                    .ThenBy(m => m.order)
                    .Select(m => m.middleware).ToArray(),
                InputTransformations = _inputTransformationsByName.Values.OrderBy(t => t.Order).ToArray()
            };
        }
    }
}