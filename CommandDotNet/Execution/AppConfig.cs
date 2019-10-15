using System;
using System.Collections.Generic;
using System.Threading;
using CommandDotNet.Builders;
using CommandDotNet.Help;
using CommandDotNet.Parsing;
using CommandDotNet.Rendering;
using CommandDotNet.Tokens;

namespace CommandDotNet.Execution
{
    /// <summary>The application configuration</summary>
    public class AppConfig
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
            NameTransformation = nameTransformation ?? ((memberName, overrideName, kind) => overrideName ?? memberName);
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
    }
}