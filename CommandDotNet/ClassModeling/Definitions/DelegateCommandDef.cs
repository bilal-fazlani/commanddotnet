using System;
using System.Collections.Generic;
using System.Reflection;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;

namespace CommandDotNet.ClassModeling.Definitions;

/// <summary>This can be used for methods passed into </summary>
internal class DelegateCommandDef(string name, Delegate handlerDelegate, AppConfig appConfig) : ICommandDef
{
    public string Name { get; } = name;
    public string SourcePath => handlerDelegate.Method.FullName(includeNamespace: true);
    public Type? CommandHostClassType { get; } = null;
    public ICustomAttributeProvider CustomAttributes => handlerDelegate.Method;
    public bool IsExecutable => true;
    public bool HasInterceptor => false;
    public IReadOnlyCollection<ICommandDef> SubCommands { get; } = new List<ICommandDef>().AsReadOnly();
    public IMethodDef? InterceptorMethodDef { get; } = null;
    public IMethodDef InvokeMethodDef { get; } = new MethodDef(handlerDelegate.Method, appConfig, false);

    public override string ToString() => $"Delegate:{SourcePath} > {Name}";
}