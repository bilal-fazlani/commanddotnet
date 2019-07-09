using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.ClassModeling;
using CommandDotNet.Extensions;

namespace CommandDotNet.Parsing
{
    internal class ValueMachine
    {
        private readonly AppSettings _appSettings;
        private readonly ParserFactory _parserFactory;

        public ValueMachine(AppSettings appSettings)
        {
            _appSettings = appSettings;
            _parserFactory = new ParserFactory(_appSettings);
        }

        public object GetValue(ArgumentInfo argumentInfo)
        {
            PromptForValue(argumentInfo);
            
            //when user has provided a value
            if (argumentInfo.ValueInfo.HasValue && argumentInfo.ValueInfo.Value != null)
            {
                //parse value
                IParser parser = _parserFactory.CreateInstance(argumentInfo);
                return parser.Parse(argumentInfo);
            }

            //when value not present but method parameter has a default value defined
            if (!argumentInfo.DefaultValue.IsNullValue())
            {
                //use default parameter or property value
                return argumentInfo.DefaultValue;
            }

            //when there no value from input and no default value, return default value of the type
            return argumentInfo.Type.GetDefaultValue();
        }

        private void PromptForValue(ArgumentInfo argumentInfo)
        {
            if (!_appSettings.PromptForArgumentsIfNotProvided
                || !(argumentInfo is OperandArgumentInfo operandInfo)
                || operandInfo.ValueInfo.HasValue
                || !operandInfo.DefaultValue.IsNullValue())
            {
                return;
            }

            List<string> inputs;
            if (operandInfo.Arity.AllowsZeroOrMore())
            {
                _appSettings.Out.Write($"{operandInfo.Name} ({operandInfo.TypeDisplayName}) [separate values by space]: ");
                inputs = Console.ReadLine()?.Split(' ').ToList() ?? new List<string>();
            }
            else
            {
                _appSettings.Out.Write($"{operandInfo.Name} ({operandInfo.TypeDisplayName}): ");
                inputs = new List<string>{ Console.ReadLine() };
            }

            var values = operandInfo.ValueInfo.Values;
            values.Clear();
            values.AddRange(inputs);
        }
    }
}