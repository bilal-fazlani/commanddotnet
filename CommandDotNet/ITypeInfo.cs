using System;

namespace CommandDotNet
{
    public interface ITypeInfo
    {
        Type Type { get; }
        Type UnderlyingType { get; }
        string DisplayName { get; set; }
    }
}