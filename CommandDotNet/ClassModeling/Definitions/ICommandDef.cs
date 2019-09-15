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
        bool HasInterceptor { get; }
        IReadOnlyCollection<ICommandDef> SubCommands { get; }
        IMethodDef InterceptorMethodDef { get; }
        IMethodDef InvokeMethodDef { get; }
        Command Command { get; set; }
    }
}