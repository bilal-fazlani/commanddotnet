using System.Collections.Generic;
using System.Linq;
using CommandDotNet.MicrosoftCommandLineUtils;

namespace CommandDotNet.Models
{
    internal class ValueInfo
    {
        private readonly CommandArgument _commandArgument;
        private readonly CommandOption _commandOption;

        private readonly bool _isArgument;

        public ValueInfo(IParameter parameter)
        {
            if (parameter is CommandArgument argument)
            {
                _isArgument = true;
                _commandArgument = argument;
            }
            else
            {
                _commandOption = (CommandOption) parameter;
            }
        }

        internal bool HasValue => _isArgument ? _commandArgument.Values.Any() : _commandOption.HasValue();

        internal List<string> Values
        {
            get => _isArgument ? _commandArgument?.Values : _commandOption?.Values;
            set
            {
                if (_isArgument)
                {
                    _commandArgument.Values = value;
                }
                else
                {
                    _commandOption.Values = value;
                }
            }
        }

        internal string Value => _isArgument ? _commandArgument?.Value : _commandOption?.Value();

        public override string ToString()
        {
            return string.Join(", ", _isArgument ? _commandArgument?.Values : _commandOption?.Values);
        }
    }
}