using System.Collections.Generic;
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

        public ParseEvents ParseEvents { get; }
        public BuildEvents BuildEvents { get; }
        public IContextData ContextData { get; }

        internal IReadOnlyCollection<ExecutionMiddleware> MiddlewarePipeline { get; set; }
        internal IReadOnlyCollection<TokenTransformation> TokenTransformations { get; set; }

        public AppConfig(AppSettings appSettings, IConsole console, 
            IDependencyResolver dependencyResolver, IHelpProvider helpProvider,
            ParseEvents parseEvents, BuildEvents buildEvents, IContextData contextData)
        {
            AppSettings = appSettings;
            Console = console;
            DependencyResolver = dependencyResolver;
            HelpProvider = helpProvider;
            ParseEvents = parseEvents;
            BuildEvents = buildEvents;
            ContextData = contextData;
        }
    }
}