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
    public sealed class Operand : IArgument
    {
        private Command _parent;
        private object _value;
        private readonly ValueProxy _valueProxy;

        [Obsolete("Use ctor without 'Command parent' parameter")]
        public Operand(
            string name,
            Command parent,
            TypeInfo typeInfo,
            IArgumentArity arity,
            string definitionSource = null,
            ICustomAttributeProvider customAttributes = null,
            ValueProxy valueProxy = null)
            : this(name, typeInfo, arity, definitionSource, customAttributes, valueProxy)
        {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        public Operand(
            string name,
            TypeInfo typeInfo,
            IArgumentArity arity,
            string definitionSource = null,
            ICustomAttributeProvider customAttributes = null,
            ValueProxy valueProxy = null)
        {
            _valueProxy = valueProxy;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            TypeInfo = typeInfo ?? throw new ArgumentNullException(nameof(typeInfo));
            Arity = arity ?? throw new ArgumentNullException(nameof(arity));
            DefinitionSource = definitionSource;
            Aliases = new[] {name};
            CustomAttributes = customAttributes ?? NullCustomAttributeProvider.Instance;
        }

        public string Name { get; }
        public string Description { get; set; }

        /// <summary>The <see cref="ITypeInfo"/> for this argument</summary>
        public ITypeInfo TypeInfo { get; set; }

        /// <summary>The <see cref="IArgumentArity"/> for this argument, describing how many values are allowed.</summary>
        public IArgumentArity Arity { get; set; }

        [Obsolete("Use Default instead. This enable middleware and custom help providers to report the source of a default value")]
        public object DefaultValue
        {
            get => Default?.Value; 
            set => Default = value == null
                ? null
                : new ArgumentDefault($"{nameof(Operand)}.{nameof(DefaultValue)}", "", value);
        }

        /// <summary>The default value for this argument</summary>
        public ArgumentDefault Default { get; set; }

        /// <summary>
        /// The allowed values for this argument, as defined by an <see cref="IAllowedValuesTypeDescriptor"/> for this type.
        /// i.e. enum arguments will list all values in the enum.
        /// </summary>
        public IReadOnlyCollection<string> AllowedValues { get; set; }

        /// <summary>
        /// The text values provided as input.
        /// Will be empty if no values were provided.<br/>
        /// Sources provided by this framework can be found at <see cref="Constants.InputValueSources"/>
        /// </summary>
        public ICollection<InputValue> InputValues { get; } = new List<InputValue>();

        /// <summary>The parsed and converted value for the argument to be passed to a method</summary>
        public object Value
        {
            get => _valueProxy == null ? _value : _valueProxy.Getter();
            set
            {
                if (_valueProxy == null)
                {
                    _value = value;
                }
                else
                {
                    _valueProxy.Setter(value);
                }
            }
        }

        /// <summary>The <see cref="Command"/> that hosts this <see cref="Operand"/></summary>
        public Command Parent
        {
            get => _parent;
            set
            {
                if (_parent != null && _parent != value)
                {
                    throw new InvalidConfigurationException($"{nameof(Parent)} is already assigned for {this}.  Current={_parent} New={value}");
                }
                _parent = value;
            }
        }

        /// <summary>The aliases defined for this argument</summary>
        public IReadOnlyCollection<string> Aliases { get; }

        /// <summary>The source that defined this argument</summary>
        public string DefinitionSource { get; }

        /// <summary>The attributes defined on the parameter or property that define this argument</summary>
        public ICustomAttributeProvider CustomAttributes { get; }

        /// <summary>The services used by middleware and associated with this argument</summary>
        public IServices Services { get; } = new Services();

        public override string ToString()
        {
            return $"Operand: {Name} ({DefinitionSource})";
        }
        
        public static bool operator ==(Operand x, Operand y) => (object)x == (object)y;

        public static bool operator !=(Operand x, Operand y) => !(x == y);

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