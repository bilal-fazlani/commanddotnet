// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using CommandDotNet.ClassModeling.Definitions;
using CommandDotNet.Execution;

namespace CommandDotNet
{
    public class Option : IArgument
    {
        private readonly HashSet<string> _aliases;
        
        public Option(string template, IArgumentArity arity, IEnumerable<string> aliases = null, ICustomAttributeProvider customAttributeProvider = null)
        {
            Template = template;
            Arity = arity;
            CustomAttributes = customAttributeProvider ?? NullCustomAttributeProvider.Instance;

            var argumentTemplate = new ArgumentTemplate(template);
            LongName = argumentTemplate.LongName;
            ShortName = argumentTemplate.ShortName;

            TypeInfo = new TypeInfo {DisplayName = argumentTemplate.TypeDisplayName};

            _aliases = aliases == null
                ? new HashSet<string>()
                : new HashSet<string>(aliases);
            if (!LongName.IsNullOrWhitespace()) _aliases.Add(LongName);
            if (!ShortName.IsNullOrWhitespace()) _aliases.Add(ShortName);
        }

        public string Name => LongName ?? ShortName;
        public string Description { get; set; }
        
        public ITypeInfo TypeInfo { get; set; }

        public IArgumentArity Arity { get; set; }
        public object DefaultValue { get; set; } = DBNull.Value;
        public IReadOnlyCollection<string> AllowedValues { get; set; }

        public string Template { get; }
        public string ShortName { get; }
        public string LongName { get; }

        public bool Inherited { get; set; }

        /// <summary>
        /// True when option is not defined in class model
        /// but is instead added via middleware.<br/>
        /// eg. Help and Version
        /// </summary>
        public bool IsSystemOption { get; set; }

        public IReadOnlyCollection<string> Aliases => _aliases;

        public ICustomAttributeProvider CustomAttributes { get; }

        public IContextData ContextData { get; } = new ContextData();

        public override string ToString()
        {
            return $"Option: {new ArgumentTemplate(LongName, ShortName, TypeInfo.DisplayName)}";
        }

        private bool Equals(Option other)
        {
            return string.Equals(LongName, other.LongName) 
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
                var hashCode = (LongName != null ? LongName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ShortName != null ? ShortName.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}