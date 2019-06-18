using System.Collections.Generic;
using System.Linq;
using CommandDotNet.MicrosoftCommandLineUtils;

namespace CommandDotNet.Models
{
    internal class ValueInfo
    {
        private readonly CommandOperand _commandOperand;
        private readonly CommandOption _commandOption;

        private readonly bool _isOperand;

        public ValueInfo(IArgument argument)
        {
            if (argument is CommandOperand operand)
            {
                _isOperand = true;
                _commandOperand = operand;
            }
            else
            {
                _commandOption = (CommandOption) argument;
            }
        }

        internal bool HasValue => _isOperand ? _commandOperand.Values.Any() : _commandOption.HasValue();

        internal List<string> Values
        {
            get => _isOperand ? _commandOperand?.Values : _commandOption?.Values;
            set
            {
                if (_isOperand)
                {
                    _commandOperand.Values = value;
                }
                else
                {
                    _commandOption.Values = value;
                }
            }
        }

        internal string Value => _isOperand ? _commandOperand?.Value : _commandOption?.Value();

        public override string ToString()
        {
            return string.Join(", ", _isOperand ? _commandOperand?.Values : _commandOption?.Values);
        }
    }
}