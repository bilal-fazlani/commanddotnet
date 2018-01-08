using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CommandDotNet.Exceptions;
using CommandDotNet.Models;

namespace CommandDotNet
{
    public static class ValueMachine
    {
        public static object GetValue(ArgumentInfo argumentInfo)
        {
            Type argType = argumentInfo.Type;

            //when value is present
            if (argumentInfo.ValueInfo.HasValue && !string.IsNullOrWhiteSpace(argumentInfo.ValueInfo.Value))
            {
                if (argType == typeof(char) || argType == typeof(char?))
                {
                    return GetChar(argumentInfo);
                }

                if (typeof(List<char>).IsAssignableFrom(argType))
                {
                    return argumentInfo.ValueInfo.Values.Select(value => GetChar(argumentInfo)).ToList();
                }

                if (argType == typeof(int) || argType == typeof(int?))
                {
                    return GetInt(argumentInfo);
                }

                if (typeof(List<int>).IsAssignableFrom(argType))
                {
                    return argumentInfo.ValueInfo.Values.Select(value => GetInt(argumentInfo)).ToList();
                }

                if (argType == typeof(long) || argType == typeof(long?))
                {
                    return GetLong(argumentInfo);
                }

                if (typeof(List<long>).IsAssignableFrom(argType))
                {
                    return argumentInfo.ValueInfo.Values.Select(value => GetLong(argumentInfo)).ToList();
                }

                if (argType == typeof(double) || argType == typeof(double?))
                {
                    return GetDouble(argumentInfo);
                }

                if (typeof(List<double>).IsAssignableFrom(argType))
                {
                    return argumentInfo.ValueInfo.Values.Select(value => GetDouble(argumentInfo)).ToList();
                }

                if (argType == typeof(bool) || argType == typeof(bool?))
                {
                    return GetBoolean(argumentInfo);
                }

                if (typeof(List<bool>).IsAssignableFrom(argType))
                {
                    return argumentInfo.ValueInfo.Values.Select(value => GetBoolean(argumentInfo)).ToList();
                }

                if (argType == typeof(string))
                {
                    return argumentInfo.ValueInfo.Value;
                }

                if (typeof(List<string>).IsAssignableFrom(argType))
                {
                    return argumentInfo.ValueInfo.Values;
                }

                if (argType.IsEnum)
                {
                    try
                    {
                        return Enum.Parse(argType, argumentInfo.ValueInfo.Value);
                    }
                    catch (ArgumentException)
                    {
                        ThrowParsingException<object>(argumentInfo);
                    }
                }

                throw new ValueParsingException(
                    $"Unsupported parameter type: {argType.FullName}");
            }

            //when value not present but method parameter has a default value defined
            if (argumentInfo.DefaultValue != DBNull.Value && argumentInfo.DefaultValue != null)
            {
                return argumentInfo.DefaultValue;
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