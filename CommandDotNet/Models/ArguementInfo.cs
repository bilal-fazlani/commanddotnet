using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.Attributes;
using Microsoft.Extensions.CommandLineUtils;

namespace CommandDotNet.Models
{
    public class ArguementInfo
    {
        private readonly ICustomAttributeProvider _attributeProvider;
        private readonly AppSettings _settings;

        public ArguementInfo(AppSettings settings)
        {
            _settings = settings;
        }
        
        private ArguementInfo(string longName, Type type, ICustomAttributeProvider attributeProvider, AppSettings settings) : this(settings)
        {
            _attributeProvider = attributeProvider;
            
            LongName = longName;
            Type = type;
            CommandOptionType = GetCommandOptionType();
            TypeDisplayName = GetTypeDisplayName();
            AnnotatedDescription = GetAnnotatedDescription();
            
            Template = GetTemplate(longName);
        }
        
        public ArguementInfo(ParameterInfo parameterInfo, AppSettings settings) 
            : this(parameterInfo.Name, parameterInfo.ParameterType, parameterInfo, settings)
        {
            DefaultValue = parameterInfo.DefaultValue;
            Required = GetIsParameterRequired(parameterInfo);
            Details = GetDetails();
            EffectiveDescription = GetEffectiveDescription();
        }
        
        public ArguementInfo(PropertyInfo propertyInfo, AppSettings settings) 
            : this(propertyInfo.Name, propertyInfo.PropertyType, propertyInfo, settings)
        {
            DefaultValue = DBNull.Value;
            Required = GetIsOptionRequired(propertyInfo);
            Details = GetDetails();
            EffectiveDescription = GetEffectiveDescription();
        }

        public string LongName { get; set; }
        public Type Type { get; set;}
        public CommandOptionType CommandOptionType { get; set;}
        public bool Required { get; set;}
        public object DefaultValue { get; set;}
        public string TypeDisplayName { get; set;}
        public string Details { get;  set;}
        public string AnnotatedDescription { get;  set;}
        public string EffectiveDescription { get;  set;}
        public string Template { get;  set;}
        
        private bool GetIsParameterRequired(ParameterInfo parameterInfo)
        {
            ArguementAttribute descriptionAttribute = parameterInfo.GetCustomAttribute<ArguementAttribute>(false);
            
            if(descriptionAttribute != null && Type == typeof(string))
            {
                if(parameterInfo.HasDefaultValue & descriptionAttribute.RequiredString) 
                    throw new Exception($"String parameter '{LongName}' can't be 'Required' and have a default value at the same time");
                
                return descriptionAttribute.RequiredString;
            }
            
            if (descriptionAttribute != null && Type != typeof(string) && descriptionAttribute.RequiredString)
            {
                throw new Exception("RequiredString can only me used with a string type parameter");
            }
            
            return parameterInfo.ParameterType.IsValueType
                   && parameterInfo.ParameterType.IsPrimitive
                   && !parameterInfo.HasDefaultValue;
        }
        
        private bool GetIsOptionRequired(PropertyInfo propertyInfo)
        {
            ArguementAttribute descriptionAttribute = propertyInfo.GetCustomAttribute<ArguementAttribute>(false);
            
            if(descriptionAttribute != null && Type == typeof(string))
            {
                return descriptionAttribute.RequiredString;
            }
            
            if (descriptionAttribute != null && Type != typeof(string) && descriptionAttribute.RequiredString)
            {
                throw new Exception("RequiredString can only me used with a string type parameter");
            }

            return propertyInfo.PropertyType.IsValueType
                   && propertyInfo.PropertyType.IsPrimitive;
        }
        
        private string GetAnnotatedDescription()
        {
            ArguementAttribute descriptionAttribute = _attributeProvider.GetCustomAttribute<ArguementAttribute>();
            return descriptionAttribute?.Description;
        }
        
        private string GetTypeDisplayName()
        {
            if (Type.Name == "String") return Type.Name;
            
            if (Nullable.GetUnderlyingType(Type) == typeof(bool))
            {
                return "Flag";
            }

            if (typeof(IEnumerable).IsAssignableFrom(Type))
            {
                return $"{Type.GetGenericArguments().SingleOrDefault()?.Name} (Multiple)";
            }
            
            return Nullable.GetUnderlyingType(Type)?.Name ?? Type.Name;
        }

        private string GetDetails()
        {
            return $"{this.GetTypeDisplayName()}{(this.Required ? " | Required" : null)}" +
                   $"{(this.DefaultValue != DBNull.Value ? " | Default value: "+ DefaultValue : null)}";
        }

        private string GetEffectiveDescription()
        {
            return _settings.ShowParameterDetails
                ? string.Format("{0}{1}", Details.PadRight(50) , AnnotatedDescription)
                : AnnotatedDescription;
        }
        
        private CommandOptionType GetCommandOptionType()
        {
            if (typeof(IEnumerable).IsAssignableFrom(Type) && Type != typeof(string))
            {
                return CommandOptionType.MultipleValue;
            }
                
            if(Type.IsAssignableFrom(typeof(bool?)))
            {
                return CommandOptionType.NoValue;
            }

            return CommandOptionType.SingleValue;
        }
        
        private string GetTemplate(string name)
        {
            return _attributeProvider.GetCustomAttribute<ArguementAttribute>()?.Template ??
                   $"--{name}";
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case ArguementInfo arguementInfo:
                    return arguementInfo.Template == this.Template;
                case CommandOption commandOption:
                    return commandOption.Template == this.Template;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Template.GetHashCode();
        }
    }
}