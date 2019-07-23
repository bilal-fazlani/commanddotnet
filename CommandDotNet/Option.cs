// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using CommandDotNet.Execution;

namespace CommandDotNet
{
    public class Option : IOption
    {
        private readonly HashSet<string> _aliases;

        public Option(string template, IArgumentArity arity, IEnumerable<string> aliases = null)
        {
            Template = template;
            Arity = arity;

            var argumentTemplate = new ArgumentTemplate(template);
            Name = argumentTemplate.Name;
            ShortName = argumentTemplate.ShortName;
            TypeInfo = new TypeInfo {DisplayName = argumentTemplate.TypeDisplayName};

            _aliases = aliases == null
                ? new HashSet<string>()
                : new HashSet<string>(aliases);
            if (!Name.IsNullOrWhitespace()) _aliases.Add(Name);
            if (!ShortName.IsNullOrWhitespace()) _aliases.Add(ShortName);
        }

        public string Name { get; }
        public string Description { get; set; }
        
        public ITypeInfo TypeInfo { get; set; }

        public IArgumentArity Arity { get; set; }
        public object DefaultValue { get; set; } = DBNull.Value;
        public List<string> AllowedValues { get; set; }

        public string Template { get; }
        public string ShortName { get; }

        public bool Inherited { get; set; }

        /// <summary>True when option is help or version</summary>
        public bool IsSystemOption { get; set; }

        public IEnumerable<string> Aliases => _aliases;

        public IContextData ContextData { get; } = new ContextData();

        public override string ToString()
        {
            return $"Option: {new ArgumentTemplate(Name, ShortName, TypeInfo.DisplayName)}";
        }

        private bool Equals(Option other)
        {
            return string.Equals(Name, other.Name) 
                   && string.Equals(ShortName, other.ShortName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((Option) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ShortName != null ? ShortName.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}