using JetBrains.Annotations;

namespace CommandDotNet.Builders;

[PublicAPI]
public class CommandBuilder(Command command) : ICommandBuilder
{
    public Command Command { get; } = command;

    public void AddSubCommand(Command command) => Command.AddArgumentNode(command);

    public void AddArgument(IArgument argument) => Command.AddArgumentNode(argument);
}