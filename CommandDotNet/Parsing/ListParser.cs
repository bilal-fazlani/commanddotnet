using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Extensions;
using CommandDotNet.TypeDescriptors;

namespace CommandDotNet.Parsing;

internal class ListParser(Type type, Type underlyingType, 
    IArgumentTypeDescriptor argumentTypeDescriptor)
    : IParser
{
    public object? Parse(IArgument argument, IEnumerable<string> values)
    {
        // TODO: when _type & values is IEnumerable but not ICollection
        //       DO NOT enumerate values here as it could be a stream.
        var listInstance = type.IsArray
            ? new ArrayList()
            : values is ICollection<string> || type.IsCollection()
                ? CreateGenericList()
                : null;

        if (listInstance == null)
        {
            if (underlyingType == typeof(string))
            {
                return values;
            }

            // must create delegate of correct type to invoke command method
            // while casting only as the stream is consumed
            Func<IEnumerable, object> f = Enumerable.Cast<object>;
            var cast = f.Method.GetGenericMethodDefinition().MakeGenericMethod(underlyingType);
            var enumerable = values.Select(v => argumentTypeDescriptor.ParseString(argument, v));
            return cast.Invoke(null, [enumerable]);
        }

        foreach (string stringValue in values)
        {
            listInstance.Add(argumentTypeDescriptor.ParseString(argument, stringValue));
        }

        return type.IsArray 
            ? ((ArrayList)listInstance).ToArray(underlyingType)
            : listInstance;
    }

    private IList CreateGenericList()
    {
        var listType = typeof(List<>).MakeGenericType(underlyingType);
        return (IList) Activator.CreateInstance(listType)!;
    }
}