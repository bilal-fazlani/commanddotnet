namespace CommandDotNet.Builders
{
    public class CommandBuilder : ICommandBuilder
    {
        public Command Command { get; }

        public CommandBuilder(Command command) => Command = command;

        public void AddSubCommand(Command command) => Command.AddArgumentNode(command);

        public void AddArgument(IArgument argument) => Command.AddArgumentNode(argument);
    }
}