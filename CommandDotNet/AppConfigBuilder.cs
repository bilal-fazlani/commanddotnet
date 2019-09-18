using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CommandDotNet.Builders;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using CommandDotNet.Help;
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

        /// <summary>
        /// Configures the app to use the resolver to create instances of
        /// properties decorated with <see cref="InjectPropertyAttribute"/>
        /// </summary>
        public IDependencyResolver DependencyResolver { get; set; }

        /// <summary>Replace the internal help provider with given help provider</summary>
        public IHelpProvider CustomHelpProvider { get; set; }

        /// <summary>Replace the internal system console with provided console</summary>
        public IConsole Console { get; set; } = new SystemConsole();

        /// <summary>
        /// This CancellationToken will be shared via the <see cref="CommandContext"/>
        /// Set it to ensure all middleware can subscribe to a cancellation.
        /// </summary>
        public CancellationToken CancellationToken { get; set; } = CancellationToken.None;

        public BuildEvents BuildEvents { get; } = new BuildEvents();
        public TokenizationEvents TokenizationEvents { get; } = new TokenizationEvents();

        /// <summary>
        /// Services registered for the lifetime of the application.<br/>
        /// Use to store configurations for use by middleware.<br/>
        /// </summary>
        public Services Services { get; } = new Services();

        /// <summary>
        /// Resolvers functions registered here are available to inject into constructors, interceptor methods and command methods.<br/>
        /// Types must be resolvable from the <see cref="CommandContext"/><br/>
        /// Default types: <see cref="CommandContext"/>, <see cref="IConsole"/>, <see cref="CancellationToken"/>
        /// </summary>
        public readonly Dictionary<Type, Func<CommandContext, object>> ParameterResolversByType = new Dictionary<Type, Func<CommandContext, object>>
        {
            [typeof(CommandContext)] = ctx => ctx,
            [typeof(IConsole)] = ctx => ctx.Console,
            [typeof(CancellationToken)] = ctx => ctx.AppConfig.CancellationToken
        };

        /// <summary>
        /// Adds the transformation to the list of transformations applied to tokens
        /// before they are parsed into commands and arguments
        /// </summary>
        public AppConfigBuilder UseTokenTransformation(string name, int order, Func<CommandContext, TokenCollection, TokenCollection> transformation)
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

        internal AppConfig Build(AppSettings appSettings)
        {
            var helpProvider = CustomHelpProvider ?? HelpTextProviderFactory.Create(appSettings);

            return new AppConfig(appSettings, Console, DependencyResolver, helpProvider, TokenizationEvents, BuildEvents, Services, CancellationToken, ParameterResolversByType)
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