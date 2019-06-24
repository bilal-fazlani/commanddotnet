using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Extensions;
using CommandDotNet.MicrosoftCommandLineUtils;

namespace CommandDotNet.Models
{
    internal class ValueInfo
    {

        private readonly ISettableArgument _argument;

        private readonly bool _isOperand;

        public ValueInfo(ISettableArgument argument)
        {
            _argument = argument ?? throw new ArgumentNullException(nameof(argument));
        }

        internal bool HasValue => _argument.Values.Any();

        internal List<string> Values
        {
            get => _argument.Values;
            set => _argument.SetValues(value);
        }

        internal string Value => _argument.Values?.FirstOrDefault();

        public override string ToString()
        {
            return _argument.Values.ToCsv(", ");
        }
    }
}