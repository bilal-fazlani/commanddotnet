using System;
using CommandDotNet.Exceptions;
using CommandDotNet.Models;

namespace CommandDotNet.Parsing
{
    internal class SingleValueParser : IParser
    {
        private readonly Type _underyingType;

        public SingleValueParser(Type underyingType)
        {
            _underyingType = underyingType;
        }

        public dynamic ParseString(string value, bool implicitBoolean, string typeDisplayName)
        {
            try
            {
                if (_underyingType == typeof(string))
                {
                    return value;
                }

                if (_underyingType.IsEnum)
                {
                    return Enum.Parse(_underyingType, value);
                }

                if (_underyingType == typeof(char))
                {
                    return char.Parse(value);
                }

                if (_underyingType == typeof(bool))
                {
                    if(implicitBoolean)
                    {
                        return value == "on";
                    }

                    return bool.Parse(value);
                }

                if (_underyingType == typeof(double))
                {
                    return double.Parse(value);
                }

                if (_underyingType == typeof(long))
                {
                    return long.Parse(value);
                }

                if (_underyingType == typeof(int))
                {
                    return int.Parse(value);
                }
            }
            catch (FormatException)
            {
                throw new ValueParsingException($"'{value}' is not a valid {typeDisplayName}");
            }
            catch (ArgumentException)
            {
                throw new ValueParsingException($"'{value}' is not a valid {typeDisplayName}");
            }
            
            throw new AppRunnerException($"type : {_underyingType} is not supported");
        }
        
        public dynamic Parse(ArgumentInfo argumentInfo)
        {
            return ParseString(argumentInfo.ValueInfo.Value, argumentInfo.IsImplicit, argumentInfo.TypeDisplayName);
        }
    }
}