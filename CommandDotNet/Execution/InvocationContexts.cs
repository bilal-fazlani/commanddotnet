using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Extensions;

namespace CommandDotNet.Execution
{
    public class InvocationContexts
    {
        public ICollection<InvocationContext> AncestorInterceptors { get; } = new List<InvocationContext>();
        public InvocationContext TargetCommand { get; set; }

        public IEnumerable<InvocationContext> All => AncestorInterceptors.Concat(TargetCommand.ToEnumerable());
    }

    public class InvocationContext
    {
        public object Instance { get; set; }
        public Command Command { get; set; }
        public IInvocation Invocation { get; set; }
    }
}