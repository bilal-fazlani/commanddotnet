using System.Collections.Generic;
using System.Reflection;

namespace CommandDotNet.Execution
{
    public interface IInvocation
    {
        IReadOnlyCollection<IArgument> Arguments { get; }
        IReadOnlyCollection<ParameterInfo> Parameters { get; }
        object[] ParameterValues { get; }
        object Invoke(CommandContext commandContext, object instance);
    }
}