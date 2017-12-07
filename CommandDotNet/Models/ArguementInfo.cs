using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;
using CommandDotNet.Attributes;
using Microsoft.Extensions.CommandLineUtils;

namespace CommandDotNet.Models
{
    public class ArgumentInfo
    {
        private readonly ParameterInfo _parameterInfo;
        private readonly AppSettings _settings;

        public ArgumentInfo(AppSettings settings)
        {
            _settings = settings;
        }
        
        public ArgumentInfo(ParameterInfo parameterInfo, AppSettings settings) 
            : this(settings)
        {
            _parameterInfo = parameterInfo;
            
            Name = parameterInfo.Name;
            Type = parameterInfo.ParameterType;
            Implicit = GetImplicit();
            CommandOptionType = GetCommandOptionType();
            TypeDisplayName = GetTypeDisplayName();
            AnnotatedDescription = GetAnnotatedDescription();
            Template = GetTemplate(parameterInfo);
            DefaultValue = parameterInfo.DefaultValue;
            Required = GetIsParameterRequired(parameterInfo);
            Details = GetDetails();
            EffectiveDescription = GetEffectiveDescription();
        }

        public string Name { get; set; }
        public Type Type { get; set;}
        public CommandOptionType CommandOptionType { get; set;}
        public bool Required { get; set;}
        public object DefaultValue { get; set;}
        public string TypeDisplayName { get; set;}
        public string Details { get;  set;}
        public string AnnotatedDescription { get;  set;}
        public string EffectiveDescription { get;  set;}
        public string Template { get;  set;}
        public bool Implicit { get; set; }

        private bool GetImplicit()
        {
            return (_parameterInfo.GetCustomAttribute<ArgumentAttribute>()?.ImplicitBoolean) ?? false;
        }

        private bool GetIsParameterRequired(ParameterInfo parameterInfo)
        {
            if (Implicit) return false;
            
            ArgumentAttribute descriptionAttribute = parameterInfo.GetCustomAttribute<ArgumentAttribute>(false);
            
            if(descriptionAttribute != null && Type == typeof(string))
            {
                if(parameterInfo.HasDefaultValue & descriptionAttribute.RequiredString) 
                    throw new Exception($"String parameter '{Name}' can't be 'Required' and have a default value at the same time");
                
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
        
        private string GetAnnotatedDescription()
        {
            ArgumentAttribute descriptionAttribute = _parameterInfo.GetCustomAttribute<ArgumentAttribute>();
            return descriptionAttribute?.Description;
        }
        
        private string GetTypeDisplayName()
        {
            if (Type.Name == "String") return Type.Name;

            if (Implicit)
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
                
            return Implicit ? CommandOptionType.NoValue : CommandOptionType.SingleValue;
        }
        
        private string GetTemplate(ParameterInfo parameterInfo)
        {
            ArgumentAttribute attribute = parameterInfo.GetCustomAttribute<ArgumentAttribute>(false);

            if (!string.IsNullOrWhiteSpace(attribute?.LongName) || !string.IsNullOrWhiteSpace(attribute?.ShortName))
            {
                StringBuilder sb = new StringBuilder();
                bool longNameAdded = false;

                if (!string.IsNullOrWhiteSpace(attribute.LongName))
                {
                    sb.Append($"--{attribute.LongName}");
                    longNameAdded = true;
                }
                
                if (!string.IsNullOrWhiteSpace(attribute.ShortName))
                {
                    if (longNameAdded)
                    {
                        sb.Append(" | ");
                    }
                    sb.Append($"-{attribute.ShortName}");
                }

                return sb.ToString();
            }

            return $"--{Name}";
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case ArgumentInfo argumentInfo:
                    return argumentInfo.Template == this.Template;
                case CommandOption commandOption:
                    return commandOption.Template == this.Template;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Template.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Name} : {Details} : {Template}";
        }
    }
}