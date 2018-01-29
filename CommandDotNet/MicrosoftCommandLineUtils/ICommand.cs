using System.Collections.Generic;

namespace CommandDotNet.MicrosoftCommandLineUtils
{
    public interface ICommand
    {
        string Name { get; }
        string FullName { get;  }
        string Description { get; }
        bool ShowInHelpText { get; }
        string ExtendedHelpText { get; }
        IEnumerable<CommandOption> GetOptions();
        string GetFullCommandName();
        HashSet<CommandOption> Options { get; }
        HashSet<CommandArgument> Arguments { get; }
        List<CommandLineApplication> Commands { get; }
        CommandOption OptionHelp { get; }
    }
}