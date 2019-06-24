using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.Extensions;
using CommandDotNet.MicrosoftCommandLineUtils;
using CommandDotNet.TypeDescriptors;

namespace CommandDotNet.Models
{
    public abstract class ArgumentInfo : INameAndDescription
    {
        private readonly IArgumentTypeDescriptor _typeDescriptor;

        protected ArgumentInfo(ParameterInfo parameterInfo, AppSettings settings)
            : this(settings,
                parameterInfo,
                parameterInfo.Name,
                parameterInfo.ParameterType,
                parameterInfo.DefaultValue)
        {
        }

        protected ArgumentInfo(PropertyInfo propertyInfo, AppSettings settings)
            : this(settings,
                propertyInfo,
                propertyInfo.Name,
                propertyInfo.PropertyType,
                GetDefaultValue(propertyInfo))
        {
            IsPartOfModel = true;
            ModelType = propertyInfo.DeclaringType;
        }

        private ArgumentInfo(
            AppSettings settings,
            ICustomAttributeProvider attributeProvider,
            string propertyOrParameterName,
            Type type,
            object defaultValue)
        {
            Settings = settings;

            AttributeProvider = attributeProvider;
            PropertyOrParameterName = propertyOrParameterName;
            Type = type;
            DefaultValue = defaultValue;
            
            UnderlyingType = GetUnderlyingType(Type);
            
            _typeDescriptor = Settings.ArgumentTypeDescriptors
                .GetDescriptorOrThrow(UnderlyingType);
        }

        protected AppSettings Settings { get; }
        protected ICustomAttributeProvider AttributeProvider { get; }


        public string Name { get; protected set; }
        public string Description { get; protected set; }

        public Type Type { get; }

        /// <summary>
        /// In cases where <see cref="ArgumentInfo.Type"/> is List or Nullable,
        /// this will be the generic type that input will be converted to.
        /// In all other cases, it will be the same as <see cref="ArgumentInfo.Type"/>
        /// </summary>
        public Type UnderlyingType { get; }
        
        public object DefaultValue { get; }

        [Obsolete("Use Arity.MaximumNumberOfValues > 1 instead")]
        public bool IsMultipleType => Arity.AllowsZeroOrMore();

        public IArgumentArity Arity { get; set; }
        public string PropertyOrParameterName { get; }
        public bool IsPartOfModel { get; }
        public Type ModelType { get; }
        internal ValueInfo ValueInfo { get; private set; }

        public virtual bool IsImplicit => false;
        
        public string TypeDisplayName => _typeDescriptor.GetDisplayName(this);

        public List<string> AllowedValues => (_typeDescriptor as IAllowedValuesTypeDescriptor)
            ?.GetAllowedValues(this)
            .ToList();

        [Obsolete("use PropertyOrParameterName instead")]
        public string PropertyOrArgumentName => PropertyOrParameterName;

        [Obsolete("Use Description instead")]
        public string AnnotatedDescription => Description;
        
        internal void SetValueInfo(ISettableArgument argument)
        {
            ValueInfo = new ValueInfo(argument);
        }

        private static object GetDefaultValue(PropertyInfo propertyInfo)
        {
            object instance = Activator.CreateInstance(propertyInfo.DeclaringType);
            object defaultValue = propertyInfo.GetValue(instance);
            if (Equals(propertyInfo.PropertyType.GetDefaultValue(), defaultValue))
            {
                return DBNull.Value;
            }

            return defaultValue;
        }

        private static Type GetUnderlyingType(Type type)
        {
            return Nullable.GetUnderlyingType(type) 
                   ?? type.GetListUnderlyingType() 
                   ?? type;
        }
    }
}