using System.Reflection;
using CommandDotNet.Attributes;
using CommandDotNet.Extensions;

namespace CommandDotNet.Models
{
    internal class CommandParameterInfo : ArgumentInfo
    {
        public CommandParameterInfo(ParameterInfo attributeProvider, AppSettings settings) : base(attributeProvider, settings)
        {
            AnnotatedDescription = GetAnnotatedDescription();
        }
        
        public CommandParameterInfo(PropertyInfo attributeProvider, AppSettings settings) : base(attributeProvider, settings)
        {
            AnnotatedDescription = GetAnnotatedDescription();
        }

        public string Name => AttributeProvider.GetCustomAttribute<ArgumentAttribute>()?.Name ??
                                   PropertyOrArgumentName.ChangeCase(Settings.Case);
        
        private string GetAnnotatedDescription()
        {
            return AttributeProvider.GetCustomAttribute<ArgumentAttribute>()?.Description;
        }

        public override string ToString()
        {
            return $"{Name} | '{ValueInfo?.Value ?? "empty"}' | {TypeDisplayName}";
        }
    }
}