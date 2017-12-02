using System;
using System.Collections.Generic;
using System.Globalization;
using CommandDotNet.Exceptions;
using CommandDotNet.Models;
using Microsoft.Extensions.CommandLineUtils;

namespace CommandDotNet
{
    public class ValueMachine
    {
        public static object GetValue(KeyValuePair<ArguementInfo, CommandOption> data)
        {
            Type argType = data.Key.Type;
            
            //when value is present
            if (data.Value.HasValue() && !string.IsNullOrWhiteSpace(data.Value.Value()))
            {
                if (argType == typeof(int) || argType == typeof(int?))
                {
                    bool isInt = int.TryParse(data.Value.Value(), NumberStyles.Integer, new NumberFormatInfo(), out int integerValue);
                    if (isInt) return integerValue;
                    ThrowParsingException<int>(data, argType);
                }

                if (argType == typeof(long) || argType == typeof(long?))
                {
                    bool isLong = long.TryParse(data.Value.Value(), NumberStyles.Integer, new NumberFormatInfo(), out long longValue);
                    if (isLong) return longValue;
                    ThrowParsingException<long>(data, argType);
                }

                if (argType == typeof(double) || argType == typeof(double?))
                {
                    bool isDouble = double.TryParse(data.Value.Value(), NumberStyles.AllowDecimalPoint, new NumberFormatInfo(), out double doubleValue);
                    if (isDouble) return doubleValue;
                    ThrowParsingException<double>(data, argType);
                }


                if (argType == typeof(bool) || argType == typeof(bool?))
                {
                    bool isBool = bool.TryParse(data.Value.Value(), out bool boolValue);
                    if (isBool) return boolValue;
                    ThrowParsingException<bool>(data, argType);
                }

                if (argType == typeof(string))
                {
                    string stringValue = data.Value.Value();
                    return stringValue;
                }
                
                throw new ValueParsingException($"Unsupported parameter type: {argType.FullName} for parameter {data.Key.LongName}");
            }
            
            //when value not present but method parameter has a default value defined
            if (data.Key.DefaultValue != DBNull.Value && data.Key.DefaultValue != null)
            {
                return data.Key.DefaultValue;
            }
            
            //when there no value from inut and no default value, return default value of the type
            return GetDefault(argType);
        }

        private static void ThrowParsingException<T>(KeyValuePair<ArguementInfo, CommandOption> data, Type argType)
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