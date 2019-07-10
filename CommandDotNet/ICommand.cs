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
        IEnumerable<IOption> GetOptions(bool includeInherited = true);

        ICustomAttributeProvider CustomAttributeProvider { get; }
        IContextData ContextData { get; }
        IOption FindOption(string alias);
    }
}