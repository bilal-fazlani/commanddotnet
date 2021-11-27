using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Extensions;
using CommandDotNet.TypeDescriptors;

namespace CommandDotNet.Parsing
{
    internal class ListParser : IParser
    {
        private readonly Type _type;
        private readonly Type _underlyingType;
        private readonly IArgumentTypeDescriptor _argumentTypeDescriptor;

        public ListParser(Type type, Type underlyingType, IArgumentTypeDescriptor argumentTypeDescriptor)
        {
            _type = type;
            _underlyingType = underlyingType;
            _argumentTypeDescriptor = argumentTypeDescriptor;
        }

        public object? Parse(IArgument argument, IEnumerable<string> values)
        {
            // TODO: when _type & values is IEnumerable but not ICollection
            //       DO NOT enumerate values here as it could be a stream.
            var listInstance = _type.IsArray
                ? new ArrayList()
                : values is ICollection<string> || _type.IsCollection()
                    ? CreateGenericList()
                    : null;

            if (listInstance == null)
            {
                if (_underlyingType == typeof(string))
                {
                    return values;
                }

                // must create delegate of correct type to invoke command method
                // while casting only as the stream is consumed
                Func<IEnumerable, object> f = Enumerable.Cast<object>;
                var cast = f.Method.GetGenericMethodDefinition().MakeGenericMethod(_underlyingType);
                var enumerable = values.Select(v => _argumentTypeDescriptor.ParseString(argument, v));
                return cast.Invoke(null, new[] { enumerable });
            }

            foreach (string stringValue in values)
            {
                listInstance.Add(_argumentTypeDescriptor.ParseString(argument, stringValue));
            }

            return _type.IsArray 
                ? ((ArrayList)listInstance).ToArray(_underlyingType)
                : listInstance;
        }

        private IList CreateGenericList()
        {
            var listType = typeof(List<>).MakeGenericType(_underlyingType);
            return (IList) Activator.CreateInstance(listType)!;
        }
    }
}