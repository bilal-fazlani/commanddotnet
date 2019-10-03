using System;
using System.Collections.Generic;
using CommandDotNet.Builders;

namespace CommandDotNet.ClassModeling.Definitions
{
    internal interface ICommandDef: ICustomAttributesContainer
    {
        string Name { get; }
        Type CommandHostClassType { get; }
        bool IsExecutable { get; }
        bool HasInterceptor { get; }
        IReadOnlyCollection<ICommandDef> SubCommands { get; }
        IMethodDef InterceptorMethodDef { get; }
        IMethodDef InvokeMethodDef { get; }
        Command Command { get; set; }
    }
}