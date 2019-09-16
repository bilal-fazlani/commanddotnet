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
        public Type CommandHostClassType { get; }
        public ICustomAttributeProvider CustomAttributeProvider => _method;
        public bool IsExecutable => true;
        public bool HasInterceptor => false;
        public IReadOnlyCollection<ICommandDef> SubCommands { get; } = new List<ICommandDef>().AsReadOnly();
        public IMethodDef InterceptorMethodDef { get; } = NullMethodDef.Instance;
        public IMethodDef InvokeMethodDef { get; }
        public Command Command { get; set; }
        
        public MethodCommandDef(MethodInfo method, Type commandHostClassType, AppConfig appConfig)
        {
            _method = method;

            Name = method.BuildName(appConfig);
            CommandHostClassType = commandHostClassType;
            InvokeMethodDef = new MethodDef(method, appConfig);
        }
    }
}