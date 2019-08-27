using System.Collections.Generic;
using System.Threading;
using CommandDotNet.Builders;
using CommandDotNet.Help;
using CommandDotNet.Parsing;
using CommandDotNet.Rendering;
using CommandDotNet.Tokens;

namespace CommandDotNet.Execution
{
    public class AppConfig
    {
        public AppSettings AppSettings { get; }
        public IConsole Console { get; }
        public IDependencyResolver DependencyResolver { get; }
        public IHelpProvider HelpProvider { get; }

        public TokenizationEvents TokenizationEvents { get; }
        public BuildEvents BuildEvents { get; }
        public IServices Services { get; }
        public CancellationToken CancellationToken { get; }

        internal IReadOnlyCollection<ExecutionMiddleware> MiddlewarePipeline { get; set; }
        internal IReadOnlyCollection<TokenTransformation> TokenTransformations { get; set; }

        public AppConfig(AppSettings appSettings, IConsole console,
            IDependencyResolver dependencyResolver, IHelpProvider helpProvider,
            TokenizationEvents tokenizationEvents, BuildEvents buildEvents, IServices services,
            CancellationToken cancellationToken)
        {
            AppSettings = appSettings;
            Console = console;
            DependencyResolver = dependencyResolver;
            HelpProvider = helpProvider;
            TokenizationEvents = tokenizationEvents;
            BuildEvents = buildEvents;
            Services = services;
            CancellationToken = cancellationToken;
        }
    }
}