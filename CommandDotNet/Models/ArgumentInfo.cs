using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.Extensions;
using CommandDotNet.MicrosoftCommandLineUtils;
using CommandDotNet.TypeDescriptors;

namespace CommandDotNet.Models
{
    public abstract class ArgumentInfo
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
            string propertyOrArgumentName,
            Type type,
            object defaultValue)
        {
            Settings = settings;

            AttributeProvider = attributeProvider;
            PropertyOrArgumentName = propertyOrArgumentName;
            Type = type;
            DefaultValue = defaultValue;
            
            IsMultipleType = GetIsMultipleType();
            UnderlyingType = GetUnderlyingType(Type);
            
            _typeDescriptor = Settings.ArgumentTypeDescriptors
                .GetDescriptorOrThrow(UnderlyingType);
        }

        protected AppSettings Settings { get; private set; }
        protected ICustomAttributeProvider AttributeProvider { get; private set; }
        
        public Type Type { get; private set; }

        /// <summary>
        /// In cases where <see cref="ArgumentInfo.Type"/> is List or Nullable,
        /// this will be the generic type that input will be converted to.
        /// In all other cases, it will be the same as <see cref="ArgumentInfo.Type"/>
        /// </summary>
        public Type UnderlyingType { get; }
        
        public object DefaultValue { get; }
        
        public string AnnotatedDescription { get; internal set; }
        public bool IsMultipleType { get; }
        public string PropertyOrArgumentName { get; }
        public bool IsPartOfModel { get; }
        public Type ModelType { get; }
        internal ValueInfo ValueInfo { get; private set; }

        public virtual bool IsImplicit => false;
        
        public string TypeDisplayName => _typeDescriptor.GetDisplayName(this);

        public List<string> AllowedValues => (_typeDescriptor as IAllowedValuesTypeDescriptor)
            ?.GetAllowedValues(this)
            .ToList();

        private bool GetIsMultipleType()
        {
            return Type != typeof(string) && Type.IsCollection();
        }
        
        internal void SetValue(IParameter parameter)
        {
            ValueInfo = new ValueInfo(parameter);
        }
        
        protected abstract string GetAnnotatedDescription();
        
        private static object GetDefaultValue(PropertyInfo propertyInfo)
        {
            object instance = Activator.CreateInstance(propertyInfo.DeclaringType);
            object defaultValue = propertyInfo.GetValue(instance);
            if (object.Equals(propertyInfo.PropertyType.GetDefaultValue(), defaultValue))
            {
                return DBNull.Value;
            }

            return defaultValue;
        }

        private static Type GetUnderlyingType(Type type)
        {
            return type.GetNullableUnderlyingType() 
                   ?? type.GetListUnderlyingType() 
                   ?? type;
        }
    }
}