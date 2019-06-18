using System;
using System.Collections.Generic;
using System.Reflection;

namespace CommandDotNet.MicrosoftCommandLineUtils
{
    public interface ICommand
    {
        string Name { get; }
        string Description { get; }
        bool ShowInHelpText { get; }
        string ExtendedHelpText { get; }
        IEnumerable<CommandOption> GetOptions();
        [Obsolete("This was used solely for help.  The functionality has been moved to help providers.")]
        string GetFullCommandName();
        HashSet<CommandOption> Options { get; }
        HashSet<CommandOperand> Operands { get; }
        ICommand Parent { get; }
        List<ICommand> Commands { get; }
        CommandOption OptionHelp { get; }
        ICustomAttributeProvider CustomAttributeProvider { get; }
    }
}