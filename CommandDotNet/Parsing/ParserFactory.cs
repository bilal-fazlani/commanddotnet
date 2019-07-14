using System;

namespace CommandDotNet.Parsing
{
    internal class ParserFactory
    {
        private readonly AppSettings _appSettings;

        public ParserFactory(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public IParser CreateInstance(IArgument argument)
        {
            return argument.Arity.AllowsZeroOrMore()
                ? new ListParser(GetSingleValueParser(argument.TypeInfo.UnderlyingType))
                : (IParser)GetSingleValueParser(argument.TypeInfo.Type);
        }

        internal SingleValueParser GetSingleValueParser(Type argumentType)
        {
            var descriptor = _appSettings.ArgumentTypeDescriptors.GetDescriptorOrThrow(argumentType);
            SingleValueParser singleValueParser = new SingleValueParser(descriptor);
            return singleValueParser;
        }
    }
}