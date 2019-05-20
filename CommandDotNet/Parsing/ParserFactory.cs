using System;
using System.Collections.Generic;
using CommandDotNet.Exceptions;
using CommandDotNet.Extensions;
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

        public IParser CreateInstance(ArgumentInfo argumentInfo)
        {
            return argumentInfo.IsMultipleType
                ? new ListParser(GetSingleValueParser(argumentInfo.UnderlyingType))
                : (IParser)GetSingleValueParser(argumentInfo.Type);
        }

        internal SingleValueParser GetSingleValueParser(Type argumentType)
        {
            var descriptor = _appSettings.ArgumentTypeDescriptors.GetDescriptorOrThrow(argumentType);
            SingleValueParser singleValueParser = new SingleValueParser(descriptor);
            return singleValueParser;
        }
    }
}