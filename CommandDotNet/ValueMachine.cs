using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CommandDotNet.Exceptions;
using CommandDotNet.MicrosoftCommandLineUtils;
using CommandDotNet.Models;

namespace CommandDotNet
{
    public static class ValueMachine
    {
        public static object GetValue(ArgumentInfo arguementInfo)
        {
            Type argType = arguementInfo.Type;

            //when value is present
            if (arguementInfo.ValueInfo.HasValue && !string.IsNullOrWhiteSpace(arguementInfo.ValueInfo.Value))
            {
                if (argType == typeof(char) || argType == typeof(char?))
                {
                    return GetChar(arguementInfo);
                }

                if (typeof(List<char>).IsAssignableFrom(argType))
                {
                    return arguementInfo.ValueInfo.Values.Select(value => GetChar(arguementInfo)).ToList();
                }

                if (argType == typeof(int) || argType == typeof(int?))
                {
                    return GetInt(arguementInfo);
                }

                if (typeof(List<int>).IsAssignableFrom(argType))
                {
                    return arguementInfo.ValueInfo.Values.Select(value => GetInt(arguementInfo)).ToList();
                }

                if (argType == typeof(long) || argType == typeof(long?))
                {
                    return GetLong(arguementInfo);
                }

                if (typeof(List<long>).IsAssignableFrom(argType))
                {
                    return arguementInfo.ValueInfo.Values.Select(value => GetLong(arguementInfo)).ToList();
                }

                if (argType == typeof(double) || argType == typeof(double?))
                {
                    return GetDouble(arguementInfo);
                }

                if (typeof(List<double>).IsAssignableFrom(argType))
                {
                    return arguementInfo.ValueInfo.Values.Select(value => GetDouble(arguementInfo)).ToList();
                }

                if (argType == typeof(bool) || argType == typeof(bool?))
                {
                    return GetBoolean(arguementInfo);
                }

                if (typeof(List<bool>).IsAssignableFrom(argType))
                {
                    return arguementInfo.ValueInfo.Values.Select(value => GetBoolean(arguementInfo)).ToList();
                }

                if (argType == typeof(string))
                {
                    return arguementInfo.ValueInfo.Value;
                }

                if (typeof(List<string>).IsAssignableFrom(argType))
                {
                    return arguementInfo.ValueInfo.Values;
                }

                throw new ValueParsingException(
                    $"Unsupported parameter type: {argType.FullName} for parameter {arguementInfo.Name}");
            }

            //when value not present but method parameter has a default value defined
            if (arguementInfo.DefaultValue != DBNull.Value && arguementInfo.DefaultValue != null)
            {
                return arguementInfo.DefaultValue;
            }

            //when there no value from inut and no default value, return default value of the type
            return GetDefault(argType);
        }

        private static object GetChar(ArgumentInfo data)
        {
            bool isChar = char.TryParse(data.ValueInfo.Value, out char charValue);
            if (isChar) return charValue;
            return ThrowParsingException<char>(data);
        }

        private static bool GetBoolean(ArgumentInfo data)
        {            
            if (data is CommandOptionInfo optionInfo && optionInfo.BooleanMode == BooleanMode.Implicit)
            {
                return optionInfo.ValueInfo.HasValue;
            }
            
            bool isBool = bool.TryParse(data.ValueInfo.Value, out bool boolValue);
            if (isBool) return boolValue;
            
            return ThrowParsingException<bool>(data);
        }

        private static double GetDouble(ArgumentInfo data)
        {
            bool isDouble = double.TryParse(data.ValueInfo.Value, NumberStyles.AllowDecimalPoint, new NumberFormatInfo(),
                out double doubleValue);
            if (isDouble) return doubleValue;
            return ThrowParsingException<double>(data);
        }

        private static long GetLong(ArgumentInfo data)
        {
            bool isLong = long.TryParse(data.ValueInfo.Value, NumberStyles.Integer, new NumberFormatInfo(),
                out long longValue);
            if (isLong) return longValue;
            return ThrowParsingException<long>(data);
        }

        private static int GetInt(ArgumentInfo data)
        {
            bool isInt = int.TryParse(data.ValueInfo.Value, NumberStyles.Integer, new NumberFormatInfo(),
                out int integerValue);
            if (isInt) return integerValue;
            return ThrowParsingException<int>(data);
        }

        private static T ThrowParsingException<T>(ArgumentInfo data)
        {
            throw new ValueParsingException($"'{data.ValueInfo.Value}' is not a valid {data.TypeDisplayName}");
        }

        private static object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
    }
}