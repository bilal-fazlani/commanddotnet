using System;
using System.Collections.Generic;
using System.Reflection;
using CommandDotNet.Execution;

namespace CommandDotNet.ClassModeling.Definitions
{
    internal class MethodCommandDef : ICommandDef
    {
        private readonly MethodBase _method;
        private readonly Func<CommandContext, object> _instantiate;
        private readonly MethodDef _methodDef;

        public string Name { get; }
        public ICustomAttributeProvider CustomAttributeProvider => _method;
        public bool IsExecutable => true;
        public IReadOnlyCollection<IArgumentDef> Arguments => _methodDef.Arguments;
        public IReadOnlyCollection<ICommandDef> SubCommands => new List<ICommandDef>().AsReadOnly();
        
        public object Instantiate(CommandContext commandContext)
        {
            return _instantiate.Invoke(commandContext);
        }

        public object Invoke(CommandContext commandContext, object instance)
        {
            return _methodDef.Invoke(instance);
        }

        public MethodCommandDef(MethodBase method, Func<CommandContext,object> instantiate, ExecutionConfig executionConfig)
        {
            _method = method;
            _instantiate = instantiate;
            _methodDef = new MethodDef(method, executionConfig);

            Name = method.BuildName(executionConfig);
        }
    }
}