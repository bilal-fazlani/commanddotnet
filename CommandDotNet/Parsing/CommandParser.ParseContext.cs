using System;
using System.Collections.Generic;
using CommandDotNet.Tokens;

namespace CommandDotNet.Parsing;

internal partial class CommandParser
{
    private class ParseContext
    {
        private Command _command;

        public CommandContext CommandContext { get; }
        public char? SeparatorFromDirective { get; }
        public AppSettings AppSettings => CommandContext.AppConfig.AppSettings;
        public ParseAppSettings ParseSettings => AppSettings.Parser;

        public Command Command
        {
            get => _command;
            set
            {
                _command = value ?? throw new ArgumentNullException(nameof(value));
                Operands = new OperandQueue(_command.Operands);
            }
        }
            
        public bool SubcommandsAreAllowed { get; private set; } = true;

        public Option? ExpectedOption { get; private set; }
        public Token? ExpectedOptionToken { get; private set; }
        public OperandQueue Operands { get; private set; }

        public bool IgnoreRemainingArguments { get; set; }
        public ICollection<Token> RemainingOperands { get; } = new List<Token>();

        public IParseError? ParserError { get; set; }

        public ParseContext(CommandContext commandContext, char? separatorFromDirective)
        {
            CommandContext = commandContext ?? throw new ArgumentNullException(nameof(commandContext));
            _command = commandContext.RootCommand ?? throw new ArgumentNullException(nameof(commandContext.RootCommand));
            Operands = new OperandQueue(_command.Operands);
            SeparatorFromDirective = separatorFromDirective;
        }

        public void ExpectOption(Option option, Token token)
        {
            ExpectedOption = option ?? throw new ArgumentNullException(nameof(option));
            ExpectedOptionToken = token ?? throw new ArgumentNullException(nameof(token));
            if (!option.IsInterceptorOption || option.AssignToExecutableSubcommands)
            {
                CommandArgumentParsed();
            }
        }

        public void ClearOption()
        {
            ExpectedOption = null;
            ExpectedOptionToken = null;
        }

        public void CommandArgumentParsed() => SubcommandsAreAllowed = false;
    }
}