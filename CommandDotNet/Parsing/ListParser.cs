using System;
using System.Collections;
using System.Collections.Generic;

namespace CommandDotNet.Parsing
{
    internal class ListParser : IParser
    {
        private readonly SingleValueParser _singleValueParser;

        public ListParser(SingleValueParser singleValueParser)
        {
            _singleValueParser = singleValueParser;
        }

        public object Parse(IArgument argument, List<string> values)
        {
            Type listType = typeof(List<>).MakeGenericType(argument.TypeInfo.UnderlyingType);
            IList listInstance = (IList)Activator.CreateInstance(listType);

            foreach (string stringValue in values)
            {
                dynamic value = _singleValueParser.ParseString(argument, stringValue);
                listInstance.Add(value);
            }

            return listInstance;
        }
    }
}