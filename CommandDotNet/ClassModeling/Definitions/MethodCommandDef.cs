using System;
using System.Collections.Generic;
using System.Reflection;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;

namespace CommandDotNet.ClassModeling.Definitions;

internal class MethodCommandDef(MethodInfo method, Type commandHostClassType, AppConfig appConfig)
    : ICommandDef
{
    public string Name { get; } = method.BuildName(CommandNodeType.Command, appConfig);
    public string SourcePath => method.FullName(includeNamespace: true);
    public Type CommandHostClassType { get; } = commandHostClassType;
    public ICustomAttributeProvider CustomAttributes => method;
    public bool IsExecutable => true;
    public bool HasInterceptor => false;
    public IReadOnlyCollection<ICommandDef> SubCommands { get; } = new List<ICommandDef>().AsReadOnly();
    public IMethodDef? InterceptorMethodDef { get; } = null;
    public IMethodDef InvokeMethodDef { get; } = new MethodDef(method, appConfig, false);

    public override string ToString() => $"Method:{SourcePath} > {Name}";
}