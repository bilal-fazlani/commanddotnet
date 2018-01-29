using System;
using System.Collections;
using System.Collections.Generic;
using CommandDotNet.Models;

namespace CommandDotNet.Parsing
{
    internal class ListParser : IParser
    {
        private readonly Type _underLyingType;
        private readonly SingleValueParser _singleValueParser;

        public ListParser(Type underLyingType, SingleValueParser singleValueParser)
        {
            _underLyingType = underLyingType;
            _singleValueParser = singleValueParser;
        }

        public dynamic Parse(ArgumentInfo argumentInfo)
        {
            Type listType = typeof(List<>).MakeGenericType(_underLyingType);
            IList listInstance = (IList) Activator.CreateInstance(listType);

            foreach (string stringValue in argumentInfo.ValueInfo.Values)
            {
                dynamic value = _singleValueParser.ParseString(stringValue, argumentInfo.IsImplicit);
                listInstance.Add(value);
            }

            return listInstance;
        }
    }
}