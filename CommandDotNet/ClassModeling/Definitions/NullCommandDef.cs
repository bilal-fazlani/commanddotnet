using System.Collections.Generic;
using System.Reflection;

namespace CommandDotNet.ClassModeling.Definitions
{
    internal class NullCommandDef : ICommandDef
    {
        public NullCommandDef(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public ICustomAttributeProvider CustomAttributeProvider => null;
        public bool IsExecutable => false;
        public IReadOnlyCollection<IArgumentDef> Arguments => new List<IArgumentDef>().AsReadOnly();
        public IReadOnlyCollection<ICommandDef> SubCommands => new List<ICommandDef>().AsReadOnly();
        public IMethodDef InstantiateMethodDef { get; } = NullMethodDef.Instance;
        public IMethodDef MiddlewareMethodDef { get; } = NullMethodDef.Instance;
        public IMethodDef InvokeMethodDef { get; } = NullMethodDef.Instance;
        public Command Command { get; set; }
    }
}