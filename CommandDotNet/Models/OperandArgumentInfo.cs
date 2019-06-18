using System.Reflection;
using CommandDotNet.Attributes;
using CommandDotNet.Extensions;

namespace CommandDotNet.Models
{
    internal class OperandArgumentInfo : ArgumentInfo
    {
        public OperandArgumentInfo(ParameterInfo attributeProvider, AppSettings settings) : base(attributeProvider, settings)
        {
            AnnotatedDescription = GetAnnotatedDescription();
        }
        
        public OperandArgumentInfo(PropertyInfo attributeProvider, AppSettings settings) : base(attributeProvider, settings)
        {
            AnnotatedDescription = GetAnnotatedDescription();
        }

        public string Name => AttributeProvider.GetCustomAttribute<OperandAttribute>()?.Name ??
                                   PropertyOrParameterName.ChangeCase(Settings.Case);
        
        private string GetAnnotatedDescription()
        {
            return AttributeProvider.GetCustomAttribute<OperandAttribute>()?.Description;
        }

        public override string ToString()
        {
            return $"{Name} | '{ValueInfo?.Value ?? "empty"}' | {TypeDisplayName}";
        }
    }
}