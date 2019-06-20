using System.Reflection;
using CommandDotNet.Attributes;
using CommandDotNet.Extensions;

namespace CommandDotNet.Models
{
    internal class OperandArgumentInfo : ArgumentInfo
    {
        private OperandAttribute _operandAttribute;
        private ArgumentAttribute _argumentAttribute;

        public OperandArgumentInfo(ParameterInfo attributeProvider, AppSettings settings) : base(attributeProvider, settings)
        {
            Init();
        }
        
        public OperandArgumentInfo(PropertyInfo attributeProvider, AppSettings settings) : base(attributeProvider, settings)
        {
            Init();
        }

        public string Name { get; private set; }

        private void Init()
        {
            _operandAttribute = AttributeProvider.GetCustomAttribute<OperandAttribute>();
            if (_operandAttribute == null)
            {
                _argumentAttribute = AttributeProvider.GetCustomAttribute<ArgumentAttribute>();
            }

            Name = GetName();
            AnnotatedDescription = GetAnnotatedDescription();
        }

        private string GetName() => _operandAttribute?.Name ?? _argumentAttribute?.Name
                                    ?? PropertyOrParameterName.ChangeCase(Settings.Case);

        private string GetAnnotatedDescription()
        {
            return _operandAttribute?.Description 
                ?? _argumentAttribute?.Description;
        }

        public override string ToString()
        {
            return $"{Name} | '{ValueInfo?.Value ?? "empty"}' | {TypeDisplayName}";
        }
    }
}