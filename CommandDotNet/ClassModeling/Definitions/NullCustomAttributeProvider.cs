using System;
using System.Reflection;

namespace CommandDotNet.ClassModeling.Definitions;

internal class NullCustomAttributeProvider : ICustomAttributeProvider
{
    internal static readonly NullCustomAttributeProvider Instance = new();

    private NullCustomAttributeProvider()
    {
    }

    public object[] GetCustomAttributes(bool inherit) => [];

    public object[] GetCustomAttributes(Type attributeType, bool inherit) => [];

    public bool IsDefined(Type attributeType, bool inherit) => false;
}