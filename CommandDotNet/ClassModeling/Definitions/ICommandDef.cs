using System;
using System.Collections.Generic;
using System.Reflection;

namespace CommandDotNet.ClassModeling.Definitions
{
    internal interface ICommandDef
    {
        string Name { get; }
        Type CommandHostClassType { get; }
        ICustomAttributeProvider CustomAttributeProvider { get; }
        bool IsExecutable { get; }
        IReadOnlyCollection<IArgumentDef> Arguments { get; }
        IReadOnlyCollection<ICommandDef> SubCommands { get; }
        IMethodDef MiddlewareMethodDef { get; }
        IMethodDef InvokeMethodDef { get; }
        Command Command { get; set; }
    }
}