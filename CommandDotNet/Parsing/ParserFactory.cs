using System;
using CommandDotNet.TypeDescriptors;

namespace CommandDotNet.Parsing;

internal class ParserFactory(AppSettings appSettings)
{
    public IParser CreateInstance(IArgument argument) =>
        argument.Arity.AllowsMany()
            ? new ListParser(
                argument.TypeInfo.Type, 
                argument.TypeInfo.UnderlyingType, 
                GetDescriptor(argument.TypeInfo.UnderlyingType))
            : new SingleValueParser(GetDescriptor(argument.TypeInfo.Type));

    private IArgumentTypeDescriptor GetDescriptor(Type argumentType) => 
        appSettings.ArgumentTypeDescriptors.GetDescriptorOrThrow(argumentType);
}