// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using CommandDotNet.ClassModeling.Definitions;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using CommandDotNet.TypeDescriptors;

namespace CommandDotNet
{
    public class Option : IArgument
    {
        private readonly HashSet<string> _aliases;
        
        public Option(
            string longName,
            char? shortName,
            TypeInfo typeInfo,
            IArgumentArity arity, 
            IEnumerable<string> aliases = null, 
            ICustomAttributeProvider customAttributeProvider = null, 
            bool isInterceptorOption = false)
        {
            if (longName.IsNullOrWhitespace() && shortName.IsNullOrWhitespace())
            {
                throw new ArgumentException("a long or short name is required");
            }

            TypeInfo = typeInfo ?? throw new ArgumentNullException(nameof(typeInfo));
            Arity = arity;
            IsInterceptorOption = isInterceptorOption;
            CustomAttributes = customAttributeProvider ?? NullCustomAttributeProvider.Instance;

            LongName = longName;
            ShortName = shortName;

            _aliases = aliases == null
                ? new HashSet<string>()
                : new HashSet<string>(aliases);
            if (!LongName.IsNullOrWhitespace()) _aliases.Add(LongName);
            if (!ShortName.IsNullOrWhitespace()) _aliases.Add(ShortName.ToString());
        }

        public string Name => LongName ?? ShortName.ToString();

        /// <summary>A single character that will be prefixed with a single hyphen.</summary>
        public char? ShortName { get; }

        /// <summary>The long name that will be prefixed with a double hyphen.</summary>
        public string LongName { get; }

        /// <summary>The aliases defined for this argument</summary>
        public IReadOnlyCollection<string> Aliases => _aliases;

        /// <summary>Describes the option</summary>
        public string Description { get; set; }

        /// <summary>The <see cref="ITypeInfo"/> for this argument</summary>
        public ITypeInfo TypeInfo { get; set; }

        /// <summary>The <see cref="IArgumentArity"/> for this argument, describing how many values are allowed.</summary>
        public IArgumentArity Arity { get; set; }

        /// <summary>The default value for this argument</summary>
        public object DefaultValue { get; set; } = DBNull.Value;

        /// <summary>
        /// The allowed values for this argument, as defined by an <see cref="IAllowedValuesTypeDescriptor"/> for this type.
        /// i.e. enum arguments will list all values in the enum.
        /// </summary>
        public IReadOnlyCollection<string> AllowedValues { get; set; }

        /// <summary>
        /// The text values provided in the shell.
        /// Will be null if no values were provided.
        /// Flag options will contain a bool value.
        /// </summary>
        public ICollection<string> RawValues { get; set; }

        /// <summary>If true, this option is inherited from a command interceptor method and can be specified after the target command</summary>
        public bool Inherited { get; set; }

        /// <summary>
        /// True when option is not defined in class model
        /// but is instead added via middleware.<br/>
        /// eg. Help and Version
        /// </summary>
        public bool IsMiddlewareOption { get; set; }

        /// <summary>
        /// True when the option is defined in an interceptor method,
        /// making it available for use when subcommands are executed.<br/>
        /// This helps distinguish an interceptor option from a default command option
        /// for parent commands.
        /// </summary>
        public bool IsInterceptorOption { get; }

        /// <summary>
        /// When true, the option should be shown in help.<br/>
        /// Default: true
        /// </summary>
        public bool ShowInHelp { get; set; } = true;

        /// <summary>The attributes defined on the parameter or property that define this argument</summary>
        public ICustomAttributeProvider CustomAttributes { get; }

        /// <summary>The services used by middleware for this argument</summary>
        public IServices Services { get; } = new Services();

        public override string ToString()
        {
            return $"Option: {Name}";
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