using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using CommandDotNet.Models;

namespace CommandDotNet.TypeDescriptors
{
    public class StringCtorTypeDescriptor : IArgumentTypeDescriptor
    {
        private static readonly ConcurrentDictionary<Type, Converter> cache = new ConcurrentDictionary<Type, Converter>();

        public bool CanSupport(Type type)
        {
            return GetConverter(type).CanConvert;
        }

        public string GetDisplayName(ArgumentInfo argumentInfo)
        {
            return argumentInfo.UnderlyingType.Name;
        }

        public object ParseString(ArgumentInfo argumentInfo, string value)
        {
            var typeConverter = argumentInfo.Arity.AllowsZeroOrMore()
                ? GetConverter(argumentInfo.UnderlyingType)
                : GetConverter(argumentInfo.Type);
            return typeConverter.StringConstructor.Invoke(new []{ value });
        }

        private static Converter GetConverter(Type type)
        {
            return cache.GetOrAdd(type, t =>
            {
                var stringCtor = type.GetConstructors().FirstOrDefault(c =>
                {
                    var parameterInfos = c.GetParameters();
                    return parameterInfos.Length == 1 && parameterInfos.First().ParameterType == typeof(string);
                });

                return new Converter(stringCtor);
            });
        }

        private class Converter
        {
            public bool CanConvert => StringConstructor != null;
            public ConstructorInfo StringConstructor { get; }

            public Converter(ConstructorInfo stringConstructor)
            {
                StringConstructor = stringConstructor;
            }
        }
    }
}