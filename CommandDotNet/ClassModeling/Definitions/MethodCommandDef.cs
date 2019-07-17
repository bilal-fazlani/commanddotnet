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
        public IMethodDef InstantiateMethodDef { get; }
        public IMethodDef InvokeMethodDef { get; }
        public ICommand Command { get; set; }
        
        public MethodCommandDef(MethodBase method, IMethodDef instantiateMethodDef, ExecutionConfig executionConfig)
        {
            _method = method;

            Name = method.BuildName(executionConfig);
            InstantiateMethodDef = instantiateMethodDef;
            InvokeMethodDef = new MethodDef(method, executionConfig);
            Arguments = InvokeMethodDef.ArgumentDefs;
        }
    }
}