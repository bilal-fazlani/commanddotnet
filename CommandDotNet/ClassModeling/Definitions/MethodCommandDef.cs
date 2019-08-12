using System;
using System.Collections.Generic;
using System.Reflection;
using CommandDotNet.Execution;

namespace CommandDotNet.ClassModeling.Definitions
{
    internal class MethodCommandDef : ICommandDef
    {
        private readonly MethodBase _method;

        public string Name { get; }
        public ICustomAttributeProvider CustomAttributeProvider => _method;
        public bool IsExecutable => true;
        public IReadOnlyCollection<IArgumentDef> Arguments { get; }
        public IReadOnlyCollection<ICommandDef> SubCommands { get; } = new List<ICommandDef>().AsReadOnly();
        public Func<object> InstanceFactory { get; }
        public IMethodDef MiddlewareMethodDef { get; }
        public IMethodDef InvokeMethodDef { get; }
        public Command Command { get; set; }
        
        public MethodCommandDef(MethodInfo method, Func<object> instanceFactory, IMethodDef middlewareMethodDef, AppConfig appConfig)
        {
            _method = method;

            Name = method.BuildName(appConfig);
            InstanceFactory = instanceFactory;
            MiddlewareMethodDef = middlewareMethodDef;
            InvokeMethodDef = new MethodDef(method, appConfig);
            Arguments = InvokeMethodDef.ArgumentDefs;
        }
    }
}