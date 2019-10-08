using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Extensions;

namespace CommandDotNet
{
    public static class CommandExtensions
    {
        public static bool IsRootCommand(this Command command) => command.Parent == null;

        public static Command GetRootCommand(this Command command) => command.GetParentCommands(true).Last();

        public static IEnumerable<IArgument> AllArguments(this Command command, bool includeInterceptorOptions = false)
        {
            var allLocalArgs = command.Operands.Cast<IArgument>().Concat(command.Options);

            // inherited options are already included in allLocalArgs
            return includeInterceptorOptions
                ? command.GetParentCommands()
                    .Reverse()
                    .SelectMany(c => c.Options.Where(o => o.IsInterceptorOption && !o.Inherited))
                    .Concat(allLocalArgs)
                : allLocalArgs;
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

        public static string GetPath(this IArgument argument) =>
            $"{argument.Parent.GetPath()} {argument.Name}";

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

        public static bool HelpWasRequested(this Command command) => 
            command.HasRawValues(Constants.HelpOptionName);

        public static bool ContainsArgumentNode(this Command command, string alias) =>
            command.FindArgumentNode(alias) != null;

        public static bool HasRawValues(this Command command, string alias) => 
            command.FindRawValues(alias) != null;

        public static ICollection<string> FindRawValues(this Command command, string alias) => 
            command.FindArgumentNode(alias) is IArgument argument ? argument.RawValues : null;
    }
}