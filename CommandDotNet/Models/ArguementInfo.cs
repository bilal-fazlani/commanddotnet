using System;
using System.Reflection;
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

        internal ArgumentInfo(ParameterInfo attributeProvider, AppSettings settings)
            : this(settings)
        {
            AttributeProvider = attributeProvider;
            PropertyOrArgumentName = attributeProvider.Name;
            Type = attributeProvider.ParameterType;
            DefaultValue = attributeProvider.DefaultValue;
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
            DefaultValue = propertyInfo.GetDefaultValue();
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
    }
}