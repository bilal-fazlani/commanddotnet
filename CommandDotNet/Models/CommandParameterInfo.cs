using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using CommandDotNet.Attributes;

namespace CommandDotNet.Models
{
    public class CommandParameterInfo : ArgumentInfo
    {
        public CommandParameterInfo(AppSettings settings) : base(settings)
        {
        }

        public CommandParameterInfo(ParameterInfo parameterInfo, AppSettings settings) : base(parameterInfo, settings)
        {
            TypeDisplayName = GetTypeDisplayName();
            AnnotatedDescription = GetAnnotatedDescription();
            Details = GetDetails();
            EffectiveDescription = GetEffectiveDescription();
        }

        protected override string GetDetails()
        {
            return $"{this.GetTypeDisplayName()}" +
                   $"{(this.DefaultValue != DBNull.Value ? " | Default value: " + DefaultValue : null)}";
        }
        
        protected override string GetAnnotatedDescription()
        {
            ArgumentAttribute descriptionAttribute = ParameterInfo.GetCustomAttribute<ArgumentAttribute>();
            return descriptionAttribute?.Description;
        }
        
        protected override string GetTypeDisplayName()
        {
            if (Type.Name == "String" || Type.Name == "Boolean") return Type.Name;

            if (typeof(IEnumerable).IsAssignableFrom(Type))
            {
                return $"{Type.GetGenericArguments().FirstOrDefault()?.Name} (Multiple)";
            }

            return Nullable.GetUnderlyingType(Type)?.Name ?? Type.Name;
        }

        public override string ToString()
        {
            return $"{Name} | '{ValueInfo?.Value ?? "null"}' | {Details}";
        }
    }
}