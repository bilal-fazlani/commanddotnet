using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace CommandDotNet.Execution
{
    public interface IInvocation
    {
        IReadOnlyCollection<IArgument> Arguments { get; }
        IReadOnlyCollection<ParameterInfo> Parameters { get; }
        object[] ParameterValues { get; }
        Task<int> InvokeAsMiddleware(CommandContext commandContext, object instance, Func<CommandContext, Task<int>> next);
        object Invoke(CommandContext commandContext, object instance);
    }
}