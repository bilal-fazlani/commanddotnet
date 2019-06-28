using System.Collections.Generic;

namespace CommandDotNet.Parsing
{
    public class ParserContext
    {
        public IEnumerable<InputTransformation> InputTransformations { get; internal set; }

        public bool ParseDirectiveEnabled { get; internal set; }
    }
}