using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using CommandDotNet.Attributes;
using CommandDotNet.Extensions;

namespace CommandDotNet.Models
{
    internal class CommandParameterInfo : ArgumentInfo
    {
        public CommandParameterInfo(AppSettings settings) : base(settings)
        {
        }

        public CommandParameterInfo(ParameterInfo attributeProvider, AppSettings settings) : base(attributeProvider, settings)
        {
            TypeDisplayName = GetTypeDisplayName();
            AnnotatedDescription = GetAnnotatedDescription();
        }
        
        public CommandParameterInfo(PropertyInfo attributeProvider, AppSettings settings) : base(attributeProvider, settings)
        {
            TypeDisplayName = GetTypeDisplayName();
            AnnotatedDescription = GetAnnotatedDescription();
        }

        public string Name => AttributeProvider.GetCustomAttribute<ArgumentAttribute>()?.Name ??
                                   PropertyOrArgumentName.ChangeCase(Settings.Case);        

        protected override string GetDetails()
        {
            return $"{this.GetTypeDisplayName()}" +
                   $"{(this.DefaultValue != DBNull.Value ? " | Default value: " + DefaultValue : null)}";
        }
        
        protected override string GetAnnotatedDescription()
        {
            ArgumentAttribute descriptionAttribute = AttributeProvider.GetCustomAttribute<ArgumentAttribute>();
            return descriptionAttribute?.Description;
        }
        
        protected override string GetTypeDisplayName()
        {
            return GetTypeDisplayName(Type, BooleanMode.Explicit);
        }

        public override string ToString()
        {
            return $"{Name} | '{ValueInfo?.Value ?? "empty"}' | {TypeDisplayName}";
        }
    }
}