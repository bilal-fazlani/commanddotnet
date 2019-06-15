using System.Collections.Generic;

namespace CommandDotNet.Parsing
{
    public class ParserContext
    {
        public IEnumerable<ArgumentTransformation> ArgumentTransformations { get; internal set; }

        public bool ParseDirectiveEnabled { get; internal set; }
    }
}