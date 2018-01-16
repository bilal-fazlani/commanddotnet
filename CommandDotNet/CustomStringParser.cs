using System;
using System.Collections.Generic;
using System.Globalization;
using CommandDotNet.Exceptions;
using CommandDotNet.Models;

namespace CommandDotNet
{
    public class CustomStringParser
    {
        public T ParseSingleValue<T>(string value) where T : struct
        {
            throw new NotImplementedException("not done yet");
        }
        
        public T ParseNullableValue<T>(string value) 
        {
            throw new NotImplementedException("not done yet");
        }
        
        public T ParseList<T>(IEnumerable<string> values)
        {
            throw new NotImplementedException("not done yet");
        }

        private char GetChar(ArgumentInfo argumentInfo, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return default;
            }
            
            bool isChar = char.TryParse(value, out char charValue);
            if (isChar) return charValue;
            return ThrowParsingException<char>(argumentInfo);
        }

        private static bool GetBoolean(ArgumentInfo argumentInfo, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return default;
            }
            
            if (argumentInfo is CommandOptionInfo optionInfo && optionInfo.BooleanMode == BooleanMode.Implicit)
            {
                return optionInfo.ValueInfo.HasValue;
            }
            
            bool isBool = bool.TryParse(value, out bool boolValue);
            if (isBool) return boolValue;
            
            return ThrowParsingException<bool>(argumentInfo);
        }

        private static double GetDouble(ArgumentInfo data, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return default;
            }
            
            bool isDouble = double.TryParse(value, NumberStyles.AllowDecimalPoint, new NumberFormatInfo(),
                out double doubleValue);
            if (isDouble) return doubleValue;
            return ThrowParsingException<double>(data);
        }

        private static long GetLong(ArgumentInfo data, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return default;
            }
            
            bool isLong = long.TryParse(value, NumberStyles.Integer, new NumberFormatInfo(),
                out long longValue);
            if (isLong) return longValue;
            return ThrowParsingException<long>(data);
        }

        private static int GetInt(ArgumentInfo data, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return default;
            }
            
            bool isInt = int.TryParse(value, NumberStyles.Integer, new NumberFormatInfo(),
                out int integerValue);
            if (isInt) return integerValue;
            return ThrowParsingException<int>(data);
        }
        
        private static T ThrowParsingException<T>(ArgumentInfo data)
        {
            throw new ValueParsingException($"'{data.ValueInfo.Value}' is not a valid {data.TypeDisplayName}");
        }
    }
}