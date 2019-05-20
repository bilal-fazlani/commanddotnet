using System;
using CommandDotNet.Models;
using CommandDotNet.TypeDescriptors;
using FluentAssertions;

namespace CommandDotNet.Tests.Parsing.Models
{
    public class PersonTypeDescriptor : IArgumentTypeDescriptor
    {
        public bool CanSupport(Type type)
        {
            return type == typeof(Person);
        }

        public string GetDisplayName(ArgumentInfo argumentInfo)
        {
            return "text";
        }

        public object ParseString(ArgumentInfo argumentInfo, string value)
        {
            argumentInfo.Should().NotBeNull();
            return new Person(value);
        }
    }
}