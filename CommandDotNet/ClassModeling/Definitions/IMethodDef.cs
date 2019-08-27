using System.Collections.Generic;
using CommandDotNet.Execution;

namespace CommandDotNet.ClassModeling.Definitions
{
    internal interface IMethodDef : IInvocation
    {
        IReadOnlyCollection<IArgumentDef> ArgumentDefs { get; }
    }
}