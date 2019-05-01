using System;
using System.Collections.Generic;
using CommandDotNet.Exceptions;
using CommandDotNet.Models;
using CommandDotNet.TypeDescriptors;

namespace CommandDotNet.Parsing
{
    internal class ParserFactory
    {
        private readonly AppSettings _appSettings;

        public ParserFactory(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public IParser CreateInstance(Type argumentType)
        {
            if (argumentType.IsGenericType && argumentType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                Type underLyingType = argumentType.GetGenericArguments()[0];
                return new NullableValueParser(underLyingType, GetSingleValueParser(underLyingType));
            }

            if (argumentType.IsGenericType && argumentType.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type underLyingType = argumentType.GetGenericArguments()[0];
                return new ListParser(underLyingType, GetSingleValueParser(underLyingType));
            }
            
            if (argumentType.IsValueType || argumentType == typeof(string))
            {
                return GetSingleValueParser(argumentType);
            }

            throw new AppRunnerException($"type of '{argumentType.Name}' is not supported");
        }

        internal SingleValueParser GetSingleValueParser(Type argumentType)
        {
            var descriptor = _appSettings.ArgumentTypeDescriptors.GetDescriptorOrThrow(argumentType);
            SingleValueParser singleValueParser = new SingleValueParser(descriptor);
            return singleValueParser;
        }
    }
}