using System;
using System.Collections;
using System.Collections.Generic;
using CommandDotNet.ClassModeling;

namespace CommandDotNet.Parsing
{
    internal class ListParser : IParser
    {
        private readonly SingleValueParser _singleValueParser;

        public ListParser(SingleValueParser singleValueParser)
        {
            _singleValueParser = singleValueParser;
        }

        public dynamic Parse(ArgumentInfo argumentInfo)
        {
            Type listType = typeof(List<>).MakeGenericType(argumentInfo.UnderlyingType);
            IList listInstance = (IList) Activator.CreateInstance(listType);

            foreach (string stringValue in argumentInfo.ValueInfo.Values)
            {
                dynamic value = _singleValueParser.ParseString(argumentInfo, stringValue);
                listInstance.Add(value);
            }

            return listInstance;
        }
    }
}