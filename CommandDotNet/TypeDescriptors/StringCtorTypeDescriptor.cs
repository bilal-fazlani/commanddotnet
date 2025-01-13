using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.Extensions;
using JetBrains.Annotations;

namespace CommandDotNet.TypeDescriptors;

[PublicAPI]
public class StringCtorTypeDescriptor : IArgumentTypeDescriptor
{
    private static readonly Dictionary<Type, Converter> Cache = new();

    public bool CanSupport(Type type) => 
        GetConverter(type).MethodBase is not null;

    public string GetDisplayName(IArgument argument) => 
        GetConverter(argument).MethodBase!.GetParameters().Single().Name!;

    public object? ParseString(IArgument argument, string value)
    {
        var converter = GetConverter(argument);
        return converter.StringConstructor is not null 
            ? converter.StringConstructor!.Invoke([value]) 
            : converter.ParseMethod!.Invoke(null, [value]);
    }

    private static Converter GetConverter(IArgument argument) =>
        argument.Arity.AllowsMany()
            ? GetConverter(argument.TypeInfo.UnderlyingType)
            : GetConverter(argument.TypeInfo.Type);

    private static Converter GetConverter(Type type)
    {
        return Cache.GetOrAdd(type, t =>
        {
            var stringCtor = t.GetConstructors()
                .FirstOrDefault(HasSingleRequiredStringArgument);

            if (stringCtor is not null)
            {
                return new Converter{StringConstructor = stringCtor};
            }

            // intentionally skipping TryParse because we want the error message if parse fails.
            var parseMethod = t
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(c =>
                    c.Name == "Parse"
                    && HasSingleRequiredStringArgument(c));

            return new Converter{ParseMethod = parseMethod};
        });

        static bool HasSingleRequiredStringArgument(MethodBase method)
        {
            var parameterInfos = method.GetParameters();
            return parameterInfos.Count(p => p.ParameterType == typeof(string) && !p.IsOptional) == 1;
        }
    }

    private class Converter
    {
        public MethodBase? MethodBase => (MethodBase?)StringConstructor ?? ParseMethod;
            
        public ConstructorInfo? StringConstructor { get; set; }
            
        public MethodInfo? ParseMethod { get; set; }
    }
}