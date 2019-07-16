using System;
using System.Collections;
using System.Collections.Generic;
using CommandDotNet.TypeDescriptors;

namespace CommandDotNet.Parsing
{
    internal class ListParser : IParser
    {
        private readonly IArgumentTypeDescriptor _argumentTypeDescriptor;

        public ListParser(IArgumentTypeDescriptor argumentTypeDescriptor)
        {
            _argumentTypeDescriptor = argumentTypeDescriptor;
        }

        public object Parse(IArgument argument, List<string> values)
        {
            Type listType = typeof(List<>).MakeGenericType(argument.TypeInfo.UnderlyingType);
            IList listInstance = (IList)Activator.CreateInstance(listType);

            foreach (string stringValue in values)
            {
                dynamic value = _argumentTypeDescriptor.ParseString(argument, stringValue);
                listInstance.Add(value);
            }

            return listInstance;
        }
    }
}