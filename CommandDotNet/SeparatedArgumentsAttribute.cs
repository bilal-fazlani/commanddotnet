using System;
using System.Collections.Generic;

namespace CommandDotNet
{
    /// <summary>
    /// Use this attribute to assign all arguments after '--' to
    /// parameters and properties of type <see cref="IEnumerable{T}"/>]
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public class SeparatedArgumentsAttribute : Attribute
    {

    }
}