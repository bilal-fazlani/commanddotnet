using System;
using System.Collections;
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
                    return GetChar(argumentInfo, argumentInfo.ValueInfo.Value);
                }

                if (typeof(List<char>).IsAssignableFrom(argType))
                {
                    return argumentInfo.ValueInfo.Values.Select(value => GetChar(argumentInfo, value)).ToList();
                }

                if (argType == typeof(int) || argType == typeof(int?))
                {
                    return GetInt(argumentInfo, argumentInfo.ValueInfo.Value);
                }

                if (typeof(List<int>).IsAssignableFrom(argType))
                {
                    return argumentInfo.ValueInfo.Values.Select(value => GetInt(argumentInfo, value)).ToList();
                }

                if (argType == typeof(long) || argType == typeof(long?))
                {
                    return GetLong(argumentInfo, argumentInfo.ValueInfo.Value);
                }

                if (typeof(List<long>).IsAssignableFrom(argType))
                {
                    return argumentInfo.ValueInfo.Values.Select(value => GetLong(argumentInfo, value)).ToList();
                }

                if (argType == typeof(double) || argType == typeof(double?))
                {
                    return GetDouble(argumentInfo, argumentInfo.ValueInfo.Value);
                }

                if (typeof(List<double>).IsAssignableFrom(argType))
                {
                    return argumentInfo.ValueInfo.Values.Select(value => GetDouble(argumentInfo, value)).ToList();
                }

                if (argType == typeof(bool) || argType == typeof(bool?))
                {
                    return GetBoolean(argumentInfo, argumentInfo.ValueInfo.Value);
                }

                if (typeof(List<bool>).IsAssignableFrom(argType))
                {
                    return argumentInfo.ValueInfo.Values.Select(value => GetBoolean(argumentInfo, value)).ToList();
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
                    return GetEnum(argumentInfo.ValueInfo.Value, argType, argumentInfo);
                }

                Type genericParameterType = argType.GenericTypeArguments?.SingleOrDefault(x => x.IsEnum);
                
                if (typeof(IList).IsAssignableFrom(argType) && genericParameterType != null)
                {
                    return GetEnums(argumentInfo, genericParameterType);
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

        private static object GetEnum(string value, Type enumType, ArgumentInfo argumentInfo)
        {
            try
            {
                return Enum.Parse(enumType, value);
            }
            catch (ArgumentException)
            {
                ThrowParsingException<object>(argumentInfo);
                return null; // this will never be executed
            }
        }
        
        private static object GetEnums(ArgumentInfo argumentInfo, Type enumType)
        {
            if (argumentInfo.ValueInfo.Values.Any())
            {
                Type listType = typeof(List<>).MakeGenericType(new [] { enumType } );
                IList list = (IList) Activator.CreateInstance(listType);
                var enumValues = argumentInfo.ValueInfo.Values.Select(v => GetEnum(v, enumType, argumentInfo));
                foreach (var enumValue in enumValues)
                {
                    list.Add(enumValue);
                }
                return list;
            }

            return null;
        }
        
        private static char GetChar(ArgumentInfo data, string value)
        {
            bool isChar = char.TryParse(value, out char charValue);
            if (isChar) return charValue;
            return ThrowParsingException<char>(data);
        }

        private static bool GetBoolean(ArgumentInfo data, string value)
        {            
            if (data is CommandOptionInfo optionInfo && optionInfo.BooleanMode == BooleanMode.Implicit)
            {
                return optionInfo.ValueInfo.HasValue;
            }
            
            bool isBool = bool.TryParse(value, out bool boolValue);
            if (isBool) return boolValue;
            
            return ThrowParsingException<bool>(data);
        }

        private static double GetDouble(ArgumentInfo data, string value)
        {
            bool isDouble = double.TryParse(value, NumberStyles.AllowDecimalPoint, new NumberFormatInfo(),
                out double doubleValue);
            if (isDouble) return doubleValue;
            return ThrowParsingException<double>(data);
        }

        private static long GetLong(ArgumentInfo data, string value)
        {
            bool isLong = long.TryParse(value, NumberStyles.Integer, new NumberFormatInfo(),
                out long longValue);
            if (isLong) return longValue;
            return ThrowParsingException<long>(data);
        }

        private static int GetInt(ArgumentInfo data, string value)
        {
            bool isInt = int.TryParse(value, NumberStyles.Integer, new NumberFormatInfo(),
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