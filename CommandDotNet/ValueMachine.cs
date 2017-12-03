using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CommandDotNet.Exceptions;
using CommandDotNet.Models;
using Microsoft.Extensions.CommandLineUtils;

namespace CommandDotNet
{
    public static class ValueMachine
    {
        public static object GetValue(KeyValuePair<ArguementInfo, CommandOption> data)
        {
            Type argType = data.Key.Type;
            
            //when value is present
            if (data.Value.HasValue() && !string.IsNullOrWhiteSpace(data.Value.Value()))
            {
                if (argType == typeof(int) || argType == typeof(int?))
                {
                    return GetInt(data);
                }

                if (typeof(List<int>).IsAssignableFrom(argType))
                {
                    return data.Value.Values.Select(value => GetInt(data)).ToList();
                }
                
                if (argType == typeof(long) || argType == typeof(long?))
                {
                    return GetLong(data);
                }
                
                if (typeof(List<long>).IsAssignableFrom(argType))
                {
                    return data.Value.Values.Select(value => GetLong(data)).ToList();
                }

                if (argType == typeof(double) || argType == typeof(double?))
                {
                    return GetDouble(data);
                }

                if (typeof(List<double>).IsAssignableFrom(argType))
                {
                    return data.Value.Values.Select(value => GetDouble(data)).ToList();
                }

                if (argType == typeof(bool) || argType == typeof(bool?))
                {
                    return GetBoolean(data);
                }
                
                if (typeof(List<bool>).IsAssignableFrom(argType))
                {
                    return data.Value.Values.Select(value => GetBoolean(data)).ToList();
                }

                if (argType == typeof(string))
                {
                    return data.Value.Value();
                }

                if (typeof(List<string>).IsAssignableFrom(argType))
                {
                    return data.Value.Values;
                }
                
                throw new ValueParsingException($"Unsupported parameter type: {argType.FullName} for parameter {data.Key.Name}");
            }
            
            //when value not present but method parameter has a default value defined
            if (data.Key.DefaultValue != DBNull.Value && data.Key.DefaultValue != null)
            {
                return data.Key.DefaultValue;
            }
            
            //when there no value from inut and no default value, return default value of the type
            return GetDefault(argType);
        }

        private static bool GetBoolean(KeyValuePair<ArguementInfo, CommandOption> data)
        {
            bool isBool = bool.TryParse(data.Value.Value(), out bool boolValue);
            if (isBool) return boolValue;
            return ThrowParsingException<bool>(data);
        }

        private static double GetDouble(KeyValuePair<ArguementInfo, CommandOption> data)
        {
            bool isDouble = double.TryParse(data.Value.Value(), NumberStyles.AllowDecimalPoint, new NumberFormatInfo(),
                out double doubleValue);
            if (isDouble) return doubleValue;
            return ThrowParsingException<double>(data);
        }

        private static long GetLong(KeyValuePair<ArguementInfo, CommandOption> data)
        {
            bool isLong = long.TryParse(data.Value.Value(), NumberStyles.Integer, new NumberFormatInfo(), out long longValue);
            if (isLong) return longValue;
            return ThrowParsingException<long>(data);
        }

        private static int GetInt(KeyValuePair<ArguementInfo, CommandOption> data)
        {
            bool isInt = int.TryParse(data.Value.Value(), NumberStyles.Integer, new NumberFormatInfo(), out int integerValue);
            if (isInt) return integerValue;
            return ThrowParsingException<int>(data);
        }

        private static T ThrowParsingException<T>(KeyValuePair<ArguementInfo, CommandOption> data)
        {
            throw new ValueParsingException($"'{data.Value.Value()}' is not a valid {data.Key.TypeDisplayName}");
        }

        private static object GetDefault(Type type)
        {
            if(type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
    }
}