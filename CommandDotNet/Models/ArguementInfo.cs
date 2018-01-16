using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using CommandDotNet.Extensions;
using CommandDotNet.MicrosoftCommandLineUtils;

namespace CommandDotNet.Models
{
    public abstract class ArgumentInfo
    {
        protected readonly ICustomAttributeProvider AttributeProvider;
        protected readonly AppSettings Settings;

        public bool IsPartOfModel = false;
        public Type ModelType;
        
        internal ArgumentInfo(AppSettings settings)
        {
            Settings = settings;
        }

        internal ArgumentInfo(ParameterInfo parameterInfo, AppSettings settings)
            : this(settings)
        {
            AttributeProvider = parameterInfo;
            PropertyOrArgumentName = parameterInfo.Name;
            Type = parameterInfo.ParameterType;
            DefaultValue = parameterInfo.DefaultValue;
            IsMultipleType = GetIsMultipleType();
        }

        internal ArgumentInfo(PropertyInfo propertyInfo, AppSettings settings)
            : this(settings)
        {
            IsPartOfModel = true;
            ModelType = propertyInfo.DeclaringType;
            AttributeProvider = propertyInfo;
            PropertyOrArgumentName = propertyInfo.Name;
            Type = propertyInfo.PropertyType;
            DefaultValue = GetDefaultValue(propertyInfo);
            IsMultipleType = GetIsMultipleType();
        }
        
        
        public Type Type { get; internal set; }
        
        public object DefaultValue { get; internal set; }
        public string TypeDisplayName { get; internal set; }
        public string Details { get; internal set; }
        public string AnnotatedDescription { get; internal set; }
        public string EffectiveDescription { get; internal set; }
        public bool IsMultipleType { get; }
        public string PropertyOrArgumentName { get; set; }
        internal ValueInfo ValueInfo { get; private set; }

        public bool IsImplicit =>
            this is CommandOptionInfo optionInfo && optionInfo.BooleanMode == BooleanMode.Implicit;

        private bool GetIsMultipleType()
        {
            return Type != typeof(string) && Type.IsCollection();
        }
        
        internal void SetValue(IParameter parameter)
        {
            ValueInfo = new ValueInfo(parameter);
        }

        protected abstract string GetAnnotatedDescription();

        protected abstract string GetTypeDisplayName();
        
        protected abstract string GetDetails();
        
        protected string GetEffectiveDescription()
        {
            return Settings.ShowArgumentDetails
                ? $"{Details.PadRight(Constants.PadLength)}{AnnotatedDescription}"
                : AnnotatedDescription;
        }
        
        private object GetDefaultValue(PropertyInfo propertyInfo)
        {
            object instance = Activator.CreateInstance(propertyInfo.DeclaringType);
            object defaultValue = propertyInfo.GetValue(instance);
            if (object.Equals(propertyInfo.PropertyType.GetDefaultValue(), defaultValue))
            {
                return DBNull.Value;
            }

            return defaultValue;
        }
    }
}