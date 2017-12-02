using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using CommandDotNet.Attributes;
using Microsoft.Extensions.CommandLineUtils;

namespace CommandDotNet
{
    public class CommandParameterInfo
    {
        private readonly AppSettings _settings;
        private readonly ParameterInfo _parameterInfo;

        public CommandParameterInfo(AppSettings settings)
        {
            _settings = settings;
        }
        
        public CommandParameterInfo(ParameterInfo parameterInfo, AppSettings settings)
        {
            _settings = settings;
            _parameterInfo = parameterInfo;
            
            ParameterName = parameterInfo.Name;
            ParameterType = parameterInfo.ParameterType;
            CommandOptionType = getCommandOptionType(parameterInfo);
            Required = GetIsRequired(parameterInfo);
            DefaultValue = parameterInfo.DefaultValue;
            TypeDisplayName = GetTypeDisplayName();
            ParameterDetails = GetInfo();
            AnnotatedDescription = GetDescription(parameterInfo);
            Description = GetEffectiveDescription();
        }
        
        public string ParameterName { get; set; }
        public Type ParameterType { get; set;}
        public CommandOptionType CommandOptionType { get; set;}
        public bool Required { get; set;}
        public object DefaultValue { get; set;}
        public string TypeDisplayName { get;}
        public string ParameterDetails { get;}
        public string AnnotatedDescription { get; }
       
        public string Description { get; set;}

        private CommandOptionType getCommandOptionType(ParameterInfo parameterInfo)
        {
            if (typeof(IEnumerable).IsAssignableFrom(parameterInfo.ParameterType) && parameterInfo.ParameterType != typeof(string))
            {
                return CommandOptionType.MultipleValue;
            }
                
            if(parameterInfo.ParameterType.IsAssignableFrom(typeof(bool?)))
            {
                return CommandOptionType.NoValue;
            }

            return CommandOptionType.SingleValue;
        }

        private bool GetIsRequired(ParameterInfo parameterInfo)
        {
            ParameterAttribute descriptionAttribute = parameterInfo.GetCustomAttribute<ParameterAttribute>(false);
            
            if(descriptionAttribute != null && ParameterType == typeof(string))
            {
                if(parameterInfo.HasDefaultValue & descriptionAttribute.RequiredString) 
                    throw new Exception($"String parameter '{ParameterName}' can't be 'Required' and have a default value at the same time");
                
                return descriptionAttribute.RequiredString;
            }
            
            if (descriptionAttribute != null && ParameterType != typeof(string) && descriptionAttribute.RequiredString)
            {
                throw new Exception("RequiredString can only me used with a string type parameter");
            }
            
            return parameterInfo.ParameterType.IsValueType
                   && parameterInfo.ParameterType.IsPrimitive
                   && !parameterInfo.HasDefaultValue;
        }
        
        private string GetDescription(ParameterInfo parameterInfo)
        {
            ParameterAttribute descriptionAttribute = parameterInfo.GetCustomAttribute<ParameterAttribute>(false);
            return descriptionAttribute?.Description;
        }
        
        private string GetTypeDisplayName()
        {
            if (ParameterType.Name == "String") return ParameterType.Name;
            
            if (Nullable.GetUnderlyingType(ParameterType) == typeof(bool))
            {
                return "Flag";
            }

            if (typeof(IEnumerable).IsAssignableFrom(ParameterType))
            {
                return $"{ParameterType.GetGenericArguments().SingleOrDefault()?.Name} (Multiple)";
            }
            
            return Nullable.GetUnderlyingType(ParameterType)?.Name ?? ParameterType.Name;
        }

        private string GetInfo()
        {
            return $"{this.GetTypeDisplayName()}{(this.Required ? " | Required" : null)}" +
                   $"{(this.DefaultValue != DBNull.Value ? " | Default value: "+ DefaultValue.ToString() : null)}";
        }

        private string GetEffectiveDescription()
        {
            return _settings.ShowParameterInfo
                ? string.Format("{0}{1}", ParameterDetails.PadRight(50) , AnnotatedDescription)
                : AnnotatedDescription;
        }
    }
}