using System;
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

        public IParser CreateInstance(IArgument argument)
        {
            return argument.Arity.AllowsMany()
                ? new ListParser(
                    argument.TypeInfo.Type, 
                    argument.TypeInfo.UnderlyingType, 
                    GetDescriptor(argument.TypeInfo.UnderlyingType))
                : new SingleValueParser(GetDescriptor(argument.TypeInfo.Type));
        }

        private IArgumentTypeDescriptor GetDescriptor(Type argumentType)
        {
            return _appSettings.ArgumentTypeDescriptors.GetDescriptorOrThrow(argumentType);
        }
    }
}