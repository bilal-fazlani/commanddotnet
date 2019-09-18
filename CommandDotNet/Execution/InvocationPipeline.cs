using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Extensions;

namespace CommandDotNet.Execution
{
    public class InvocationPipeline
    {
        public ICollection<InvocationStep> AncestorInterceptors { get; } = new List<InvocationStep>();
        public InvocationStep TargetCommand { get; set; }

        public IEnumerable<InvocationStep> All => AncestorInterceptors.Concat(TargetCommand.ToEnumerable());
    }
}