using System;
using System.Reflection;
using CommandDotNet.Execution;

namespace CommandDotNet.ClassModeling.Definitions
{
    internal class ParameterArgumentDef : IArgumentDef
    {
        private readonly ParameterInfo _parameterInfo;
        private readonly ExecutionConfig _executionConfig;
        private readonly Action<object> _assignValue;

        public ArgumentType ArgumentType { get; }

        public string Name { get; }

        public Type Type => _parameterInfo.ParameterType;

        public bool HasDefaultValue => _parameterInfo.HasDefaultValue;

        public object DefaultValue => _parameterInfo.DefaultValue;

        public IArgument Argument { get; set; }

        public ICustomAttributeProvider Attributes => _parameterInfo;

        public ParameterArgumentDef(
            ParameterInfo parameterInfo,
            ArgumentType argumentType,
            ExecutionConfig executionConfig,
            Action<object> assignValue)
        {
            _parameterInfo = parameterInfo ?? throw new ArgumentNullException(nameof(parameterInfo));
            _executionConfig = executionConfig ?? throw new ArgumentNullException(nameof(executionConfig));
            _assignValue = assignValue ?? throw new ArgumentNullException(nameof(assignValue));

            ArgumentType = argumentType;

            Name = parameterInfo.BuildName(executionConfig);
        }

        public void SetValue(object value)
        {
            _assignValue(value);
        }

        public override string ToString()
        {
            return $"Parameter:{_parameterInfo.Member.DeclaringType?.Name}.{_parameterInfo.Member.Name}.{_parameterInfo.Name} > {Name}({Type})";
        }
    }
}