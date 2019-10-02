namespace CommandDotNet
{
    public class CommandNodeType
    {
        public static readonly CommandNodeType Command = new CommandNodeType {IsCommand = true, _name = nameof(Command)};
        public static readonly CommandNodeType Operand = new CommandNodeType {IsOperand = true, _name = nameof(Operand)};
        public static readonly CommandNodeType Option = new CommandNodeType {IsOption = true, _name = nameof(Option)};

        private string _name;

        public bool IsCommand { get; private set; }
        public bool IsOperand { get; private set; }
        public bool IsOption { get; private set; }
        public bool IsArgument => IsOperand || IsOption;

        private CommandNodeType()
        {
        }

        public override string ToString()
        {
            return $"{nameof(CommandNodeType)}: {_name}";
        }
    }
}