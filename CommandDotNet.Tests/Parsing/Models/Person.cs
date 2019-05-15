using System;

namespace CommandDotNet.Tests.Parsing.Models
{
    public class Person
    {
        public string Name { get; }

        public Person(string name)
        {
            Name = name;
        }

        public override bool Equals(object obj)
        {
            return obj is Person other && String.Equals(Name, other.Name);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}