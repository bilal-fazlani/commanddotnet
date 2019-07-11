using System.Collections.Generic;
using CommandDotNet.Builders;
using CommandDotNet.Parsing;

namespace CommandDotNet.Execution
{
    public class ExecutionConfig
    {
        public AppSettings AppSettings { get; }
        public IDependencyResolver DependencyResolver { get; }

        public ParseEvents Events { get; } = new ParseEvents();

        internal IReadOnlyCollection<ExecutionMiddleware> MiddlewarePipeline { get; set; }
        internal IReadOnlyCollection<InputTransformation> InputTransformations { get; set; }
    }
}