using System;

namespace CommandDotNet
{
    public class TypeInfo : ITypeInfo
    {
        public Type Type { get; set; }
        public Type UnderlyingType { get; set; }
        public string DisplayName { get; set; }
    }
}