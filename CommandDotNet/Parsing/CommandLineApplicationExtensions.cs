using System;
using System.Linq;
using CommandDotNet.MicrosoftCommandLineUtils;

namespace CommandDotNet.Parsing
{
    internal static class CommandLineApplicationExtensions
    {
        internal static CommandOption FindOption(
            this CommandLineApplication app, 
            Func<CommandOption, string> optionNameToCompare, string optionName)
        {
            return app.GetOptions().SingleOrDefault(o => 
                optionName.Equals(optionNameToCompare(o), StringComparison.Ordinal));
        }
    }
}