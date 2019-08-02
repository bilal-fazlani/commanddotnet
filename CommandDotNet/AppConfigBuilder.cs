using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Builders;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using CommandDotNet.Parsing;
using CommandDotNet.Rendering;
using CommandDotNet.Tokens;

namespace CommandDotNet
{
    public class AppConfigBuilder
    {
        private readonly SortedDictionary<MiddlewareStages, List<(ExecutionMiddleware middleware, int order)>> _middlewareByStage = 
            new SortedDictionary<MiddlewareStages, List<(ExecutionMiddleware middleware, int order)>>();

        private readonly Dictionary<string, TokenTransformation> _tokenTransformationsByName = 
            new Dictionary<string, TokenTransformation>();


        private IDependencyResolver _dependencyResolver;

        internal IConsole Console { get; private set; } = new SystemConsole();

        public BuildEvents BuildEvents { get; } = new BuildEvents();
        public ParseEvents ParseEvents { get; } = new ParseEvents();
        public ContextData ContextData { get; } = new ContextData();
        
        /// <summary>Replace the internal system console with provided console</summary>
        public AppConfigBuilder UseConsole(IConsole console)
        {
            Console = console;
            return this;
        }

        /// <summary>
        /// Configures the app to use the resolver to create instances of
        /// properties decorated with <see cref="InjectPropertyAttribute"/>
        /// </summary>
        public AppConfigBuilder UseDependencyResolver(IDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver ?? throw new ArgumentNullException(nameof(dependencyResolver));
            return this;
        }

        /// <summary>
        /// Adds the transformation to the list of transformations applied to tokens
        /// before they are parsed into commands and arguments
        /// </summary>
        public AppConfigBuilder UseTokenTransformation(string name, int order, Func<TokenCollection,TokenCollection> transformation)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (transformation == null) throw new ArgumentNullException(nameof(transformation));

            if (_tokenTransformationsByName.ContainsKey(name))
            {
                throw new ArgumentException(
                    $"a transformation named {name} already exists. " +
                    $"transformations={_tokenTransformationsByName.Keys.ToOrderedCsv()}");
            }
            _tokenTransformationsByName.Add(name, new TokenTransformation(name, order, transformation));

            return this;
        }

        /// <summary>
        /// Adds the middleware to the pipeline in the specified <see cref="MiddlewareStages"/>.
        /// Use <see cref="orderWithinStage"/> to specify order in relation
        /// to other middleware within the same stage.
        /// </summary>
        public AppConfigBuilder UseMiddleware(ExecutionMiddleware middleware, MiddlewareStages stage, int? orderWithinStage = null)
        {
            var values = _middlewareByStage
                .GetOrAdd(stage, s => new List<(ExecutionMiddleware middleware, int order)>());
            
            values.Add((middleware, orderWithinStage ?? values.Count));

            return this;
        }

        public AppConfig Build(AppSettings appSettings)
        {
            return new AppConfig(appSettings, Console, _dependencyResolver, ParseEvents, BuildEvents, ContextData)
            {
                MiddlewarePipeline = _middlewareByStage
                    .SelectMany(kvp => kvp.Value.Select(v => new {stage = kvp.Key, v.order, v.middleware}) )
                    .OrderBy(m => m.stage)
                    .ThenBy(m => m.order)
                    .Select(m => m.middleware).ToArray(),
                TokenTransformations = _tokenTransformationsByName.Values.OrderBy(t => t.Order).ToArray()
            };
        }
    }
}