using System;
using CommandDotNet.Models;

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
            return argumentInfo.Arity.AllowsZeroOrMore()
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