using System.Collections.Generic;
using CommandDotNet.Builders;
using CommandDotNet.Parsing;
using CommandDotNet.Tokens;

namespace CommandDotNet.Execution
{
    public class ExecutionConfig
    {
        public AppSettings AppSettings { get; }
        public IDependencyResolver DependencyResolver { get; }

        public ParseEvents ParseEvents { get; }
        public BuildEvents BuildEvents { get; }
        internal IContextData ContextData { get; }

        internal IReadOnlyCollection<ExecutionMiddleware> MiddlewarePipeline { get; set; }
        internal IReadOnlyCollection<TokenTransformation> TokenTransformations { get; set; }

        public ExecutionConfig(AppSettings appSettings, IDependencyResolver dependencyResolver,
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