using System;
using System.Reflection;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;

namespace CommandDotNet.ClassModeling.Definitions
{
    internal class PropertyArgumentDef : IArgumentDef
    {
        private readonly PropertyInfo _propertyInfo;

        public string ArgumentDefType => "Property";
        
        public CommandNodeType CommandNodeType { get; }

        public string Name { get; }

        public string SourcePath => _propertyInfo.FullName(includeNamespace: true);

        public Type Type => _propertyInfo.PropertyType;

        public bool HasDefaultValue { get; }

        public object DefaultValue { get; }

        public IArgument Argument { get; set; }

        public ICustomAttributeProvider CustomAttributes => _propertyInfo;

        public ValueProxy ValueProxy { get; }

        public PropertyArgumentDef(
            PropertyInfo propertyInfo,
            CommandNodeType commandNodeType,
            AppConfig appConfig,
            object modelInstance)
        {
            if (modelInstance == null)
            {
                throw new ArgumentNullException(nameof(modelInstance));
            }

            _propertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));

            CommandNodeType = commandNodeType;

            Name = propertyInfo.BuildName(commandNodeType, appConfig);

            DefaultValue = propertyInfo.GetValue(modelInstance);

            ValueProxy = new ValueProxy(
                () => _propertyInfo.GetValue(modelInstance),

                value => _propertyInfo.SetValue(modelInstance, value)
            );

            // Enhancement: AppSetting.StrictDefaults: show any default values that will be used.
            //       If a value type doesn't have a default, it would be defined as a nullable type.
            //       Keeping this behavior for legacy support.
            if (DefaultValue.IsDefaultFor(propertyInfo.PropertyType))
            {
                DefaultValue = DBNull.Value;
            }

            HasDefaultValue = DefaultValue != DBNull.Value;
        }

        public override string ToString()
        {
            return $"Property:{SourcePath} > {Name}({Type})";
        }
    }
}