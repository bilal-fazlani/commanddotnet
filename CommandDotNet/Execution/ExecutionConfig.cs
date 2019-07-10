using System.Collections.Generic;
using CommandDotNet.Parsing;

namespace CommandDotNet.Execution
{
    public class ExecutionConfig
    {
        public ParserEvents Events { get; }

        public ExecutionConfig(CommandContext commandContext)
        {
            Events = new ParserEvents(commandContext);
        }

        internal IEnumerable<ExecutionMiddleware> MiddlewarePipeline { get; set; }
        internal IEnumerable<InputTransformation> InputTransformations { get; set; }
    }
}