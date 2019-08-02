using System.Collections.Generic;
using CommandDotNet.Builders;
using CommandDotNet.Parsing;
using CommandDotNet.Tokens;

namespace CommandDotNet.Execution
{
    public class AppConfig
    {
        public AppSettings AppSettings { get; }
        public IDependencyResolver DependencyResolver { get; }

        public ParseEvents ParseEvents { get; }
        public BuildEvents BuildEvents { get; }
        public IContextData ContextData { get; }

        internal IReadOnlyCollection<ExecutionMiddleware> MiddlewarePipeline { get; set; }
        internal IReadOnlyCollection<TokenTransformation> TokenTransformations { get; set; }

        public AppConfig(AppSettings appSettings, IDependencyResolver dependencyResolver,
            ParseEvents parseEvents, BuildEvents buildEvents, IContextData contextData)
        {
            AppSettings = appSettings;
            DependencyResolver = dependencyResolver;
            ParseEvents = parseEvents;
            BuildEvents = buildEvents;
            ContextData = contextData;
        }
    }
}