using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.Extensions;

namespace CommandDotNet.TypeDescriptors
{
    public class StringCtorTypeDescriptor : IArgumentTypeDescriptor
    {
        private static readonly Dictionary<Type, Converter> Cache = new Dictionary<Type, Converter>();

        public bool CanSupport(Type type)
        {
            return GetConverter(type).CanConvert;
        }

        public string GetDisplayName(IArgument argument)
        {
            return GetConverter(argument).StringConstructor.GetParameters().Single().Name;
        }

        public object ParseString(IArgument argument, string value)
        {
            return GetConverter(argument).StringConstructor.Invoke(new object[] { value });
        }

        private static Converter GetConverter(IArgument argument)
        {
            return argument.Arity.AllowsMany()
                ? GetConverter(argument.TypeInfo.UnderlyingType)
                : GetConverter(argument.TypeInfo.Type);
        }

        private static Converter GetConverter(Type type)
        {
            return Cache.GetOrAdd(type, t =>
            {
                var stringCtor = t.GetConstructors().FirstOrDefault(c =>
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