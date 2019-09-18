using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Extensions;
using CommandDotNet.Parsing;

namespace CommandDotNet.Execution
{
    public class InvocationPipeline
    {
        /// <summary>
        /// The invocations for the interceptor methods of the ancestor commands of the <see cref="TargetCommand"/>.
        /// Order is top-most parent first.
        /// </summary>
        public ICollection<InvocationStep> AncestorInterceptors { get; set; } = new List<InvocationStep>();

        /// <summary>The invocation for the <see cref="ParseResult.TargetCommand"/></summary>
        public InvocationStep TargetCommand { get; set; }

        /// <summary>
        /// Joins <see cref="AncestorInterceptors"/> and <see cref="TargetCommand"/> with top-most parent
        /// first and <see cref="TargetCommand"/> last.
        /// </summary>
        public IEnumerable<InvocationStep> All => AncestorInterceptors.Concat(TargetCommand.ToEnumerable());
    }
}