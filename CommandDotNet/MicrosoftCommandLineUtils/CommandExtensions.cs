using System.Collections.Generic;
using System.Linq;

namespace CommandDotNet.MicrosoftCommandLineUtils
{
    public static class CommandExtensions
    {
        public static bool IsRootCommand<T>(this T command) where T : ICommand
        {
            return command.Parent == null;
        }

        public static T GetRootCommand<T>(this T command) where T : ICommand
        {
            return command.GetParentCommands(true).Last();
        }

        public static IEnumerable<T> GetParentCommands<T>(this T command, bool includeCurrent = false) where T : ICommand
        {
            var startingCommand = includeCurrent ? command : command.Parent;
            for (ICommand c = startingCommand; c != null; c = c.Parent)
            {
                yield return (T)c;
            }
        }
    }
}