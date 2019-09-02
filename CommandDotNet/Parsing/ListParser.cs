using System;
using System.Collections;
using System.Collections.Generic;
using CommandDotNet.TypeDescriptors;

namespace CommandDotNet.Parsing
{
    internal class ListParser : IParser
    {
        private readonly Type _type;
        private readonly IArgumentTypeDescriptor _argumentTypeDescriptor;

        public ListParser(Type type, IArgumentTypeDescriptor argumentTypeDescriptor)
        {
            _type = type;
            _argumentTypeDescriptor = argumentTypeDescriptor;
        }

        public object Parse(IArgument argument, IEnumerable<string> values)
        {
            var listInstance = CreateGenericList();

            foreach (string stringValue in values)
            {
                listInstance.Add(_argumentTypeDescriptor.ParseString(argument, stringValue));
            }

            return listInstance;
        }

        private IList CreateGenericList()
        {
            var listType = typeof(List<>).MakeGenericType(_type);
            return (IList) Activator.CreateInstance(listType);
        }
    }
}