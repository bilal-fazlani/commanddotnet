using System;
using JetBrains.Annotations;

namespace CommandDotNet;

/// <summary>
/// Specifies a static method that will be called to generate the description dynamically at runtime.
/// The method must be static, take no parameters, and return a string.
/// </summary>
[PublicAPI]
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public class DescriptionMethodAttribute : Attribute
{
    /// <summary>
    /// Gets the name of the method to call for generating the description.
    /// </summary>
    public string MethodName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DescriptionMethodAttribute"/> class.
    /// </summary>
    /// <param name="methodName">The name of the static method to call for generating the description.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="methodName"/> is null.</exception>
    public DescriptionMethodAttribute(string methodName)
    {
        MethodName = methodName ?? throw new ArgumentNullException(nameof(methodName));
    }
}
