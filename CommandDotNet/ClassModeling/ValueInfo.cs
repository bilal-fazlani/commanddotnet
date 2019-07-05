using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Extensions;

namespace CommandDotNet.ClassModeling
{
    internal class ValueInfo
    {
        private readonly IArgument _argument;
        private readonly Action<List<string>> _setValues;

        public ValueInfo(IArgument argument, Action<List<string>> setValues)
        {
            _argument = argument ?? throw new ArgumentNullException(nameof(argument));
            _setValues = setValues ?? throw new ArgumentNullException(nameof(setValues));
        }

        internal bool HasValue => _argument.Values.Any();

        internal List<string> Values
        {
            get => _argument.Values;
            set => _setValues(value);
        }

        internal string Value => _argument.Values?.FirstOrDefault();

        public override string ToString()
        {
            return _argument.Values.ToCsv(", ");
        }
    }
}