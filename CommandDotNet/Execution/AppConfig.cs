using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CommandDotNet.Builders;
using CommandDotNet.Extensions;
using CommandDotNet.Help;
using CommandDotNet.Parsing;
using CommandDotNet.Rendering;
using CommandDotNet.Tokens;

namespace CommandDotNet.Execution
{
    /// <summary>The application configuration</summary>
    public class AppConfig : IIndentableToString
    {
        /// <summary>The application settings</summary>
        public AppSettings AppSettings { get; }

        /// <summary>
        /// The application console. Defaults from <see cref="System.Console"/>.
        /// Override in <see cref="AppRunner.Configure"/>
        /// </summary>
        public IConsole Console { get; }

        /// <summary>
        /// The application <see cref="CancellationToken"/>.
        /// Set in <see cref="AppRunner.Configure"/>
        /// </summary>
        public CancellationToken CancellationToken { get; }

        /// <summary>
        /// Services registered for the lifetime of the application.<br/>
        /// Use to store configurations for use by middleware.<br/>
        /// Add services in <see cref="AppRunner.Configure"/>
        /// </summary>
        public IServices Services { get; }

        /// <summary>
        /// The application <see cref="IDependencyResolver"/>.
        /// Set in <see cref="AppRunner.Configure"/>
        /// </summary>
        public IDependencyResolver DependencyResolver { get; }

        /// <summary>
        /// The application <see cref="IHelpProvider"/>.
        /// Configure using <see cref="CommandDotNet.AppSettings.Help"/> or
        /// provide a customer provider using <see cref="AppRunner.Configure"/>
        /// </summary>
        public IHelpProvider HelpProvider { get; }

        internal Action<OnRunCompletedEventArgs> OnRunCompleted { get; }
        internal TokenizationEvents TokenizationEvents { get; }
        internal BuildEvents BuildEvents { get; }
        internal IReadOnlyCollection<ExecutionMiddleware> MiddlewarePipeline { get; set; }
        internal IReadOnlyCollection<TokenTransformation> TokenTransformations { get; set; }
        internal Dictionary<Type, Func<CommandContext, object>> ParameterResolversByType { get; }
        internal NameTransformation NameTransformation { get; }
        internal ResolverService ResolverService { get; }

        public AppConfig(AppSettings appSettings, IConsole console,
            IDependencyResolver dependencyResolver, IHelpProvider helpProvider, 
            NameTransformation nameTransformation, Action<OnRunCompletedEventArgs> onRunCompleted,
            TokenizationEvents tokenizationEvents, BuildEvents buildEvents, IServices services,
            CancellationToken cancellationToken,
            Dictionary<Type, Func<CommandContext, object>> parameterResolversByType)
        {
            AppSettings = appSettings;
            Console = console;
            DependencyResolver = dependencyResolver;
            HelpProvider = helpProvider;
            NameTransformation = nameTransformation ?? ((attributes, memberName, overrideName, commandNodeType) => overrideName ?? memberName);
            OnRunCompleted = onRunCompleted;
            TokenizationEvents = tokenizationEvents;
            BuildEvents = buildEvents;
            Services = services;
            CancellationToken = cancellationToken;
            ParameterResolversByType = parameterResolversByType;

            ResolverService = services.GetOrAdd(() => new ResolverService());
            ResolverService.BackingResolver = dependencyResolver;
            OnRunCompleted += args => ResolverService.OnRunCompleted(args.CommandContext);
        }

        public override string ToString()
        {
            return ToString(null, 0);
        }

        public string ToString(string indent, int depth = 0)
        {
            var nl = Environment.NewLine;

            var prefix = indent.Repeat(depth);
            var prefix2 = indent.Repeat(depth + 1);

            var tokenTransformations = TokenTransformations
                .OrderBy(t => t.Order)
                .Select(t => $"{prefix2}{t.Name}({t.Order})")
                .ToCsv(nl);

            var middleware = MiddlewarePipeline
                .Select(m => $"{prefix2}{m.Method.FullName()}")
                .ToCsv(nl);

            var paramResolvers = ParameterResolversByType.Keys
                .Select(k => $"{prefix2}{k}")
                .ToCsv(nl);

            return $"{nameof(AppConfig)}:{nl}" +
                   $"{prefix}{AppSettings.ToString(indent, depth+1)}{nl}" +
                   $"{prefix}DependencyResolver: {DependencyResolver}{nl}" +
                   $"{prefix}HelpProvider: {HelpProvider}{nl}" +
                   $"{prefix}TokenTransformations:{nl}{tokenTransformations}{nl}" +
                   $"{prefix}MiddlewarePipeline:{nl}{middleware}{nl}" +
                   $"{prefix}ParameterResolvers:{nl}{paramResolvers}";
        }
    }
}