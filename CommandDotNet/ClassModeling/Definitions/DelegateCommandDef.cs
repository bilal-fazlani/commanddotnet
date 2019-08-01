using System;
using System.Collections.Generic;
using System.Reflection;
using CommandDotNet.Execution;

namespace CommandDotNet.ClassModeling.Definitions
{
    /// <summary>This can be used for methods passed into </summary>
    internal class DelegateCommandDef : ICommandDef
    {
        private readonly Delegate _delegate;

        public string Name { get; }
        public ICustomAttributeProvider CustomAttributeProvider => _delegate.Method;
        public bool IsExecutable => true;
        public IReadOnlyCollection<IArgumentDef> Arguments { get; }
        public IReadOnlyCollection<ICommandDef> SubCommands { get; } = new List<ICommandDef>().AsReadOnly();
        public IMethodDef InstantiateMethodDef { get; }
        public IMethodDef InvokeMethodDef { get; }
        public Command Command { get; set; }

        public DelegateCommandDef(string name, Delegate handlerDelegate, AppConfig appConfig)
        {
            _delegate = handlerDelegate;
            
            Name = name;
            InstantiateMethodDef = NullMethodDef.Instance;
            InvokeMethodDef = new MethodDef(handlerDelegate.Method, appConfig);
            Arguments = InvokeMethodDef.ArgumentDefs;

        }
    }
}