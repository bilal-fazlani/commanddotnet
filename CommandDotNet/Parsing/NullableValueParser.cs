using System;
using CommandDotNet.Models;

namespace CommandDotNet.Parsing
{
    internal class NullableValueParser : IParser
    {
        private readonly Type _type;
        private readonly SingleValueParser _singleValueParser;

        public NullableValueParser(Type type, SingleValueParser singleValueParser)
        {
            _type = type;
            _singleValueParser = singleValueParser;
        }
        
        public dynamic Parse(ArgumentInfo argumentInfo)
        {
            dynamic value = _singleValueParser.ParseString(argumentInfo.ValueInfo.Value, argumentInfo.IsImplicit, argumentInfo.TypeDisplayName);
            Type nullableType = typeof(Nullable<>).MakeGenericType(_type);
            object instance = Activator.CreateInstance(nullableType, new[]{value});
            return instance;
        }
    }
}