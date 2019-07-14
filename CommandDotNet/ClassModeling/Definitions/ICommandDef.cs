using System.Collections.Generic;
using System.Reflection;
using CommandDotNet.Execution;

namespace CommandDotNet.ClassModeling.Definitions
{
    internal interface ICommandDef
    {
        string Name { get; }
        ICustomAttributeProvider CustomAttributeProvider { get; }
        bool IsExecutable { get; }
        IReadOnlyCollection<IArgumentDef> Arguments { get; }
        IReadOnlyCollection<ICommandDef> SubCommands { get; }
        object Instantiate(CommandContext commandContext);
        object Invoke(CommandContext commandContext, object instance);
    }
}