using System;
using System.Reflection;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;

namespace CommandDotNet.ClassModeling.Definitions
{
    internal class PropertyArgumentDef : IArgumentDef
    {
        private readonly PropertyInfo _propertyInfo;
        private readonly object _modelInstance;

        public ArgumentType ArgumentType { get; }

        public string Name { get; }

        public Type Type => _propertyInfo.PropertyType;

        public bool HasDefaultValue { get; }

        public object DefaultValue { get; }

        public IArgument Argument { get; set; }

        public ICustomAttributeProvider Attributes => _propertyInfo;

        public PropertyArgumentDef(
            PropertyInfo propertyInfo,
            ArgumentType argumentType,
            ExecutionConfig executionConfig,
            object modelInstance)
        {
            _propertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
            _modelInstance = modelInstance ?? throw new ArgumentNullException(nameof(modelInstance));

            ArgumentType = argumentType;

            Name = propertyInfo.BuildName(executionConfig);

            DefaultValue = propertyInfo.GetValue(modelInstance);

            // Enhancement: AppSetting.StrictDefaults: show any default values that will be used.
            //       If a value type doesn't have a default, it would be defined as a nullable type.
            //       Keeping this behavior for legacy support.
            if (Equals(DefaultValue, propertyInfo.PropertyType.GetDefaultValue()))
            {
                DefaultValue = DBNull.Value;
            }

            HasDefaultValue = DefaultValue != DBNull.Value;
        }

        public void SetValue(object value)
        {
            _propertyInfo.SetValue(_modelInstance, value);
        }

        public override string ToString()
        {
            return $"Property:{_propertyInfo.DeclaringType?.Name}.{_propertyInfo.Name} > {Name}({Type})";
        }
    }
}