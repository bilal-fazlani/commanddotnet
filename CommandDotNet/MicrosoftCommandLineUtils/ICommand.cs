using System.Collections.Generic;
using System.Reflection;

namespace CommandDotNet.MicrosoftCommandLineUtils
{
    public interface ICommand : INameAndDescription
    {
        string Name { get; }
        string Description { get; }
        string ExtendedHelpText { get; }
        ICommand Parent { get; }
        IEnumerable<ICommand> Commands { get; }
        IEnumerable<IOperand> Operands { get; }
        IEnumerable<IOption> GetOptions(bool includeInherited = true);

        ICustomAttributeProvider CustomAttributeProvider { get; }
        IOption FindOption(string alias);
    }
}