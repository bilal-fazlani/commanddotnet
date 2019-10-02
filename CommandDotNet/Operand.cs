// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using CommandDotNet.ClassModeling.Definitions;
using CommandDotNet.Execution;
using CommandDotNet.TypeDescriptors;

namespace CommandDotNet
{
    public class Operand : IArgument
    {
        public Operand(string name, TypeInfo typeInfo, ICustomAttributeProvider customAttributeProvider = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            TypeInfo = typeInfo ?? throw new ArgumentNullException(nameof(typeInfo));
            Aliases = new[] {name};
            CustomAttributes = customAttributeProvider ?? NullCustomAttributeProvider.Instance;
        }

        public string Name { get; }
        public string Description { get; set; }

        /// <summary>The <see cref="ITypeInfo"/> for this argument</summary>
        public ITypeInfo TypeInfo { get; set; }

        /// <summary>The <see cref="IArgumentArity"/> for this argument, describing how many values are allowed.</summary>
        public IArgumentArity Arity { get; set; }

        /// <summary>The default value for this argument</summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// The allowed values for this argument, as defined by an <see cref="IAllowedValuesTypeDescriptor"/> for this type.
        /// i.e. enum arguments will list all values in the enum.
        /// </summary>
        public IReadOnlyCollection<string> AllowedValues { get; set; }

        /// <summary>
        /// The text values provided in the shell.
        /// Will be null if no values were provided.
        /// </summary>
        public ICollection<string> RawValues { get; set; }

        /// <summary>The aliases defined for this argument</summary>
        public IReadOnlyCollection<string> Aliases { get; }

        /// <summary>The attributes defined on the parameter or property that define this argument</summary>
        public ICustomAttributeProvider CustomAttributes { get; }

        /// <summary>The services used by middleware and associated with this argument</summary>
        public IServices Services { get; } = new Services();

        public override string ToString()
        {
            return $"Operand: {Name}";
        }

        private bool Equals(Operand other)
        {
            return string.Equals(Name, other.Name);
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

            return Equals((Operand) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}