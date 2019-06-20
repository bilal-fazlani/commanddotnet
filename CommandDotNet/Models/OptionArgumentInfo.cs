using System;
using System.Collections;
using System.Reflection;
using System.Text;
using CommandDotNet.Attributes;
using CommandDotNet.Exceptions;
using CommandDotNet.Extensions;
using CommandDotNet.MicrosoftCommandLineUtils;

namespace CommandDotNet.Models
{
    internal class OptionArgumentInfo : ArgumentInfo
    {
        private OptionAttribute _optionAttribute;

        public OptionArgumentInfo(ParameterInfo attributeProvider, AppSettings settings) : base(attributeProvider, settings)
        {
            Init();
        }
        
        public OptionArgumentInfo(PropertyInfo propertyInfo, AppSettings settings) : base(propertyInfo, settings)
        {
            Init();
        }

        public override bool IsImplicit => BooleanMode == BooleanMode.Implicit;

        public CommandOptionType CommandOptionType { get; private set; }
        
        public string Template { get; private set; }
        
        public BooleanMode BooleanMode { get; private set; }
        
        public string LongName { get; private set; }
        
        public string ShortName { get; private set; }

        public bool Inherited => _optionAttribute?.Inherited ?? false;

        private void Init()
        {
            _optionAttribute = AttributeProvider.GetCustomAttribute<OptionAttribute>();

            BooleanMode = GetBooleanMode();
            CommandOptionType = GetCommandOptionType();

            ShortName = GetShortName();
            LongName = GetLongName();

            Template = GetTemplate();

            AnnotatedDescription = GetAnnotatedDescription();
        }

        private string GetShortName()
        {
            string attributeShortName = _optionAttribute?.ShortName;
    
            if (!string.IsNullOrEmpty(attributeShortName)) //provided by user
                return attributeShortName;
    
            //use parameter name as short name 
            return PropertyOrParameterName.Length == 1 ? PropertyOrParameterName.ChangeCase(Settings.Case) : null;
        }

        private string GetLongName()
        {
            string attributeLongName = _optionAttribute?.LongName;
    
            if (!string.IsNullOrEmpty(attributeLongName)) //long name attribute provided by user
                return attributeLongName;

            //short name is not present, 
            if(string.IsNullOrEmpty(ShortName) && PropertyOrParameterName.Length > 1)
                return PropertyOrParameterName.ChangeCase(Settings.Case); //return parameter name as long name
    
            //there is no long name
            return null;
        }
        
        private BooleanMode GetBooleanMode()
        {
            OptionAttribute attribute = _optionAttribute;

            if (attribute == null || attribute.BooleanMode == BooleanMode.Unknown)
                return Settings.BooleanMode;

            return UnderlyingType == typeof(bool)
                ? attribute.BooleanMode
                : throw new AppRunnerException(
                    $"BooleanMode property is set to `{attribute.BooleanMode}` for a non boolean parameter type. " +
                    $"Property name: {PropertyOrParameterName} " +
                    $"Type : {Type.Name}");
        }
        
        private string GetTemplate()
        {
            StringBuilder sb = new StringBuilder();

            bool shortNameAdded = false;
            bool longNameAdded = false;

            if (!string.IsNullOrWhiteSpace(ShortName))
            {
                sb.Append($"-{ShortName}");
                shortNameAdded = true;
            }

            if (!string.IsNullOrWhiteSpace(LongName))
            {
                if (shortNameAdded)
                {
                    sb.Append(" | ");
                }
                
                sb.Append($"--{LongName}");
                longNameAdded = true;
            }

            if (!longNameAdded & !shortNameAdded)
            {
                throw new Exception("something went wrong: !longNameAdded & !shortNameAdded");
            }
    
            return sb.ToString();
        }
        
        private CommandOptionType GetCommandOptionType()
        {
            if (typeof(IEnumerable).IsAssignableFrom(Type) && Type != typeof(string))
            {
                return CommandOptionType.MultipleValue;
            }

            if ((Type == typeof(bool) || Type == typeof(bool?)) && BooleanMode == BooleanMode.Implicit)
            {
                return CommandOptionType.NoValue;
            }
            
            return CommandOptionType.SingleValue;
        }

        private string GetAnnotatedDescription()
        {
            return _optionAttribute?.Description;
        }
        
        public override string ToString()
        {
            return $"{PropertyOrParameterName} | '{ValueInfo?.Value ?? "empty"}' | {TypeDisplayName} | {Template}";
        }
    }
}