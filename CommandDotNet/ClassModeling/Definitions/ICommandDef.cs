using System.Collections.Generic;
using System.Reflection;

namespace CommandDotNet.ClassModeling.Definitions
{
    internal interface ICommandDef
    {
        string Name { get; }
        ICustomAttributeProvider CustomAttributeProvider { get; }
        bool IsExecutable { get; }
        IReadOnlyCollection<IArgumentDef> Arguments { get; }
        IReadOnlyCollection<ICommandDef> SubCommands { get; }
        IMethodDef InstantiateMethodDef { get; }
        IMethodDef MiddlewareMethodDef { get; }
        IMethodDef InvokeMethodDef { get; }
        Command Command { get; set; }
    }
}