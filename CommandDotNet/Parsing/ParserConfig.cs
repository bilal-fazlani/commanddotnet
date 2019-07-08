using System.Collections.Generic;

namespace CommandDotNet.Parsing
{
    public class ParserConfig
    {
        public ParserEvents Events { get; }

        public ParserConfig(ExecutionResult executionResult)
        {
            Events = new ParserEvents(executionResult);
        }

        internal IEnumerable<ExecutionMiddleware> MiddlewarePipeline { get; set; }
        internal IEnumerable<InputTransformation> InputTransformations { get; set; }
    }
}