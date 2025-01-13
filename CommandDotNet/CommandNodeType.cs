using CommandDotNet.Extensions;
using JetBrains.Annotations;

namespace CommandDotNet;

[PublicAPI]
public class CommandNodeType
{
    public static readonly CommandNodeType Command = new(nameof(Command)) {IsCommand = true};
    public static readonly CommandNodeType Operand = new(nameof(Operand)) {IsOperand = true};
    public static readonly CommandNodeType Option = new(nameof(Option)) {IsOption = true};

    private readonly string _name;

    public bool IsCommand { get; private set; }
    public bool IsOperand { get; private set; }
    public bool IsOption { get; private set; }
    public bool IsArgument => IsOperand || IsOption;

    private CommandNodeType(string name) => _name = name.ThrowIfNull();

    public override string ToString() => $"{nameof(CommandNodeType)}: {_name}";
}