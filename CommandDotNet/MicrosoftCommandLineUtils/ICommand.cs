using System;
using System.Collections.Generic;
using System.Reflection;

namespace CommandDotNet.MicrosoftCommandLineUtils
{
    public interface ICommand : INameAndDescription
    {
        string Name { get; }
        string Description { get; }
        bool ShowInHelpText { get; }
        string ExtendedHelpText { get; }
        ICommand Parent { get; }
        IEnumerable<ICommand> Commands { get; }
        IEnumerable<CommandOperand> Operands { get; }
        IEnumerable<CommandOption> GetOptions(bool includeInherited = true);
        CommandOption OptionHelp { get; }
        ICustomAttributeProvider CustomAttributeProvider { get; }

        [Obsolete("This was used solely for help.  The functionality has been moved to help providers.")]
        string GetFullCommandName();
    }
}