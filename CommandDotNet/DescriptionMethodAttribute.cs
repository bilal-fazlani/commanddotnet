using System;
using JetBrains.Annotations;

namespace CommandDotNet;

[PublicAPI]
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public class DescriptionMethodAttribute : Attribute
{

    public string MethodName { get; }

    public DescriptionMethodAttribute(string methodName)
    {
        MethodName = methodName ?? throw new ArgumentNullException(nameof(methodName));
    }
}
