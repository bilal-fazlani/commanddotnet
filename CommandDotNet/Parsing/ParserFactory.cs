using System;
using System.Collections.Generic;
using CommandDotNet.Exceptions;

namespace CommandDotNet.Parsing
{
    public static class ParserFactory
    {
        public static IParser CreateInstnace(Type argumentType)
        {
            if (argumentType.IsGenericType && argumentType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                Type underLyingType = argumentType.GetGenericArguments()[0];
                SingleValueParser singleValueParser = new SingleValueParser(underLyingType);
                return new NullableValueParser(underLyingType, singleValueParser);
            }

            if (argumentType.IsGenericType && argumentType.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type underLyingType = argumentType.GetGenericArguments()[0];
                SingleValueParser singleValueParser = new SingleValueParser(underLyingType);
                return new ListParser(underLyingType, singleValueParser);
            }
            
            if (argumentType.IsValueType || argumentType == typeof(string))
            {
                return new SingleValueParser(argumentType);
            }

            throw new AppRunnerException($"type of '{argumentType.Name}' is not supported");
        }
    }
}