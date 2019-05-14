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

        public IParser CreateInstance(Type argumentType)
        {
            var underLyingType = argumentType.GetNullableUnderlyingType();
            if (underLyingType != null)
            {
                return new NullableValueParser(underLyingType, GetSingleValueParser(underLyingType));
            }

            underLyingType = argumentType.GetListUnderlyingType();
            if (underLyingType != null)
            {
                return new ListParser(underLyingType, GetSingleValueParser(underLyingType));
            }
            
            return GetSingleValueParser(argumentType);
        }

        internal SingleValueParser GetSingleValueParser(Type argumentType)
        {
            var descriptor = _appSettings.ArgumentTypeDescriptors.GetDescriptorOrThrow(argumentType);
            SingleValueParser singleValueParser = new SingleValueParser(descriptor);
            return singleValueParser;
        }
    }
}