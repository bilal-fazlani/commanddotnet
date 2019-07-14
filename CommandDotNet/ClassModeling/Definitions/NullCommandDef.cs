using System.Collections.Generic;
using System.Reflection;
using CommandDotNet.Execution;

namespace CommandDotNet.ClassModeling.Definitions
{
    internal class NullCommandDef : ICommandDef
    {
        public string Name { get; }
        public ICustomAttributeProvider CustomAttributeProvider => null;
        public bool IsExecutable => false;
        public IReadOnlyCollection<IArgumentDef> Arguments => new List<IArgumentDef>().AsReadOnly();
        public IReadOnlyCollection<ICommandDef> SubCommands => new List<ICommandDef>().AsReadOnly();

        public NullCommandDef(string name)
        {
            Name = name;
        }

        public object Instantiate(CommandContext commandContext)
        {
            return null;
        }

        public object Invoke(CommandContext commandContext, object instance)
        {
            // TODO: this should never be hit or it should result in a help exception
            throw new AppRunnerException($"`{Name}` is not an executable command.");
        }
    }
}