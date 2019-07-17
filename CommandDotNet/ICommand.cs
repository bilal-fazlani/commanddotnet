using System.Collections.Generic;
using System.Reflection;

namespace CommandDotNet
{
    public interface ICommand : INameAndDescription
    {
        string ExtendedHelpText { get; }
        ICommand Parent { get; }
        IEnumerable<ICommand> Commands { get; }
        IEnumerable<IOperand> Operands { get; }

        ICustomAttributeProvider CustomAttributes { get; }
        IContextData ContextData { get; }
        IEnumerable<IOption> GetOptions(bool includeInherited = true);
        IOption FindOption(string alias);
    }
}