// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using CommandDotNet.Execution;

namespace CommandDotNet
{
    public class Option : IOption
    {
        public Option(string template, IArgumentArity arity)
        {
            Template = template;
            Arity = arity;

            var argumentTemplate = new ArgumentTemplate(template);
            Name = argumentTemplate.Name;
            ShortName = argumentTemplate.ShortName;
            SymbolName = argumentTemplate.SymbolName;
            TypeInfo = new TypeInfo {DisplayName = argumentTemplate.TypeDisplayName};
        }

        public string Name { get; }
        public string Description { get; set; }
        
        public ITypeInfo TypeInfo { get; set; }

        public IArgumentArity Arity { get; set; }
        public object DefaultValue { get; set; } = DBNull.Value;
        public List<string> AllowedValues { get; set; }

        public string Template { get; }
        public string ShortName { get; }
        public string SymbolName { get; }

        public bool Inherited { get; set; }

        /// <summary>True when option is help or version</summary>
        public bool IsSystemOption { get; set; }

        public IEnumerable<string> Aliases
        {
            get
            {
                if (!Name.IsNullOrWhitespace()) yield return Name;
                if (!ShortName.IsNullOrWhitespace()) yield return ShortName;
                if (!SymbolName.IsNullOrWhitespace()) yield return SymbolName;
            }
        }

        public IContextData ContextData { get; } = new ContextData();

        public override string ToString()
        {
            return $"Option: {new ArgumentTemplate(Name, ShortName, SymbolName, TypeInfo.DisplayName)}";
        }

        private bool Equals(Option other)
        {
            return string.Equals(Name, other.Name) 
                   && string.Equals(ShortName, other.ShortName) 
                   && string.Equals(SymbolName, other.SymbolName);
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
                hashCode = (hashCode * 397) ^ (SymbolName != null ? SymbolName.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}