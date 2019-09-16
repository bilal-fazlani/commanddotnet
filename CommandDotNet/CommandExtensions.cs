using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Extensions;

namespace CommandDotNet
{
    public static class CommandExtensions
    {
        public static bool IsRootCommand(this Command command)
        {
            return command.Parent == null;
        }

        public static Command GetRootCommand(this Command command)
        {
            return command.GetParentCommands(true).Last();
        }

        public static IEnumerable<Command> GetParentCommands(this Command command, bool includeCurrent = false)
        {
            var startingCommand = includeCurrent ? command : command.Parent;
            for (var c = startingCommand; c != null; c = c.Parent)
            {
                yield return c;
            }
        }

        public static string GetPath(this Command command, string separator = " ") =>
            command.GetParentCommands(true)
                .Reverse().Skip(1).Select(c => c.Name)
                .ToCsv(separator);

        public static IEnumerable<Command> GetDescendentCommands(this Command command, bool includeCurrent = false)
        {
            if (includeCurrent)
            {
                yield return command;
            }

            foreach (var child in command.Subcommands)
            {
                yield return child;
                foreach (var grandChild in child.GetDescendentCommands())
                {
                    yield return grandChild;
                }
            }
        }
    }
}