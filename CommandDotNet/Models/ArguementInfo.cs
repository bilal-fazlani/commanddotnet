using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CommandDotNet.Exceptions;
using CommandDotNet.Extensions;
using CommandDotNet.MicrosoftCommandLineUtils;

namespace CommandDotNet.Models
{
    public abstract class ArgumentInfo
    {
        protected readonly ICustomAttributeProvider AttributeProvider;
        protected readonly AppSettings Settings;
        
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
            AllowedValues = GetAllowedValues();
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
            AllowedValues = GetAllowedValues();
        }

        public Type Type { get; internal set; }
        
        public object DefaultValue { get; internal set; }
        public string TypeDisplayName { get; internal set; }
        public string AnnotatedDescription { get; internal set; }
        public bool IsMultipleType { get; }
        public string PropertyOrArgumentName { get; set; }
        public bool IsPartOfModel { get; set; }
        public Type ModelType { get; set; }
        public List<string> AllowedValues { get; set; }
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

        private List<string> GetAllowedValues()
        {
            if (Type == typeof(bool) && !IsImplicit)
            {
                return new List<string>(){"true", "false"};
            }

            if (Type.IsEnum)
            {
                return Enum.GetNames(Type).ToList();
            }

            return null;
        }
        
        protected abstract string GetAnnotatedDescription();

        protected abstract string GetTypeDisplayName();
        
        protected abstract string GetDetails();
        
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
        
        protected static string GetTypeDisplayName(Type type, BooleanMode booleanMode)
        {
            //is string
            if (type == typeof(string)) return Constants.TypeDisplayNames.Text;

            //is boolean
            if (type == typeof(bool))
            {
                if (booleanMode == BooleanMode.Implicit)
                    return Constants.TypeDisplayNames.Flag;
                return Constants.TypeDisplayNames.Boolean;
            }

            //List
            if (typeof(IEnumerable).IsAssignableFrom(type) && type.GetGenericArguments().Any())
            {
                return GetTypeDisplayName(type.GetGenericArguments().First(), booleanMode);
            }

            //is int
            if (type == typeof(int) || type == typeof(long))
            {
                return Constants.TypeDisplayNames.Number;
            }
            
            //is char
            if (type == typeof(char))
            {
                return Constants.TypeDisplayNames.Character;
            }
            
            //is double
            if (type == typeof(double))
            {
                return Constants.TypeDisplayNames.DecimalNumber;
            }

            //enum
            if (type.IsEnum)
            {
                return type.Name;
            }
            
            //nullable
            if (Nullable.GetUnderlyingType(type) != null)
            {
                return GetTypeDisplayName(Nullable.GetUnderlyingType(type), booleanMode);
            }

            throw new AppRunnerException($"type '{type.Name}' is not supported");
        }
    }
}