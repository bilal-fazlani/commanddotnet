using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;
using CommandDotNet.Attributes;
using CommandDotNet.Exceptions;
using CommandDotNet.MicrosoftCommandLineUtils;

namespace CommandDotNet.Models
{
    public class CommandOptionInfo : ArgumentInfo
    {
        public CommandOptionInfo(AppSettings settings) : base(settings)
        {
        }

        public CommandOptionInfo(ParameterInfo parameterInfo, AppSettings settings) : base(parameterInfo, settings)
        {
            BooleanMode = GetBooleanMode();
            CommandOptionType = GetCommandOptionType();
            Template = GetTemplate();
            
            TypeDisplayName = GetTypeDisplayName();
            AnnotatedDescription = GetAnnotatedDescription();
            Details = GetDetails();
            EffectiveDescription = GetEffectiveDescription();
        }
        
        public CommandOptionType CommandOptionType { get; set; }
        
        public string Template { get; set; }
        
        public BooleanMode BooleanMode { get; set; }
        
        private BooleanMode GetBooleanMode()
        {
            OptionAttribute attribute = ParameterInfo.GetCustomAttribute<OptionAttribute>();

            if (attribute == null || attribute.BooleanMode == BooleanMode.Unknown)
                return Settings.BooleanMode;

            if (Type == typeof(bool) || Type == typeof(bool?))
            {
                return attribute.BooleanMode;
            }

            throw new AppRunnerException(
                $"BooleanMode property is set to `{attribute.BooleanMode}` for a non boolean parameter type. " +
                $"Property name: {ParameterInfo.Name} " +
                $"Type : {Type.Name}");
        }
        
        private string GetTemplate()
        {
            OptionAttribute attribute = ParameterInfo.GetCustomAttribute<OptionAttribute>(false);

            StringBuilder sb = new StringBuilder();
            
            bool longNameAdded = false;
            bool shortNameAdded = false;
            
            if (!string.IsNullOrWhiteSpace(attribute?.LongName))
            {
                sb.Append($"--{attribute?.LongName}");
                longNameAdded = true;
            }

            if (!string.IsNullOrWhiteSpace(attribute?.ShortName))
            {
                if (longNameAdded)
                {
                    sb.Append(" | ");
                }

                sb.Append($"-{attribute?.ShortName}");
                shortNameAdded = true;
            }

            string defaultTemplate = Name.Length == 1 ? $"-{Name}" : $"--{Name}";
            if(!longNameAdded & !shortNameAdded) sb.Append(defaultTemplate);
            
            //if(CommandOptionType != CommandOptionType.NoValue) sb.Append($" <{attribute?.LongName ?? Name}>");
            
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

        protected override string GetDetails()
        {
            return
                $"{GetTypeDisplayName()}{(DefaultValue != DBNull.Value ? " | Default value: " + DefaultValue : null)}";
        }

        protected override string GetTypeDisplayName()
        {
            if (Type.Name == "String") return Type.Name;

            if (BooleanMode == BooleanMode.Implicit && (Type == typeof(bool) || Type == typeof(bool?)))
            {
                return "Flag";
            }

            if (typeof(IEnumerable).IsAssignableFrom(Type))
            {
                return $"{Type.GetGenericArguments().FirstOrDefault()?.Name} (Multiple)";
            }

            return Nullable.GetUnderlyingType(Type)?.Name ?? Type.Name;
        }
        
        protected override string GetAnnotatedDescription()
        {
            OptionAttribute descriptionAttribute = ParameterInfo.GetCustomAttribute<OptionAttribute>();
            return descriptionAttribute?.Description;
        }
        
        public override string ToString()
        {
            return $"{Name} | '{ValueInfo?.Value ?? "null"}' | {Details} | {Template}";
        }
    }
}