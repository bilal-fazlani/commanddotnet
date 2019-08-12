using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommandDotNet.Execution;

namespace CommandDotNet.ClassModeling.Definitions
{
    internal interface IMethodDef : IInvocation
    {
        IReadOnlyCollection<IArgumentDef> ArgumentDefs { get; }
        Task<int> InvokeAsMiddleware(CommandContext commandContext, object instance, Func<CommandContext, Task<int>> next);
    }
}