using System.Collections.Generic;
using CommandDotNet.Parsing;

namespace CommandDotNet.Execution
{
    public class ExecutionConfig
    {
        public ParserEvents Events { get; }

        public ExecutionConfig(ExecutionContext executionContext)
        {
            Events = new ParserEvents(executionContext);
        }

        internal IEnumerable<ExecutionMiddleware> MiddlewarePipeline { get; set; }
        internal IEnumerable<InputTransformation> InputTransformations { get; set; }
    }
}