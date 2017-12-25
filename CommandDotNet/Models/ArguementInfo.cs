using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CommandDotNet.Attributes;
using CommandDotNet.Exceptions;
using CommandDotNet.MicrosoftCommandLineUtils;

namespace CommandDotNet.Models
{
    public class ArgumentInfo
    {
        private readonly ParameterInfo _parameterInfo;
        private readonly AppSettings _settings;

        public ArgumentInfo(AppSettings settings)
        {
            _settings = settings;
        }

        public ArgumentInfo(ParameterInfo parameterInfo, AppSettings settings)
            : this(settings)
        {
            _parameterInfo = parameterInfo;

            Name = parameterInfo.Name;
            Type = parameterInfo.ParameterType;
            BooleanMode = GetBooleanMode();
            CommandOptionType = GetCommandOptionType();
            TypeDisplayName = GetTypeDisplayName();
            AnnotatedDescription = GetAnnotatedDescription();
            DefaultValue = parameterInfo.DefaultValue;
            Required = GetIsParameterRequired();
            Details = GetDetails();
            Template = GetTemplate(parameterInfo);
            EffectiveDescription = GetEffectiveDescription();
            IsSubject = GetIsSubject();
        }

        public string Name { get; set; }
        public Type Type { get; set; }
        public CommandOptionType CommandOptionType { get; set; }
        public bool Required { get; set; }
        public object DefaultValue { get; set; }
        public string TypeDisplayName { get; set; }
        public string Details { get; set; }
        public string AnnotatedDescription { get; set; }
        public string EffectiveDescription { get; set; }
        public string Template { get; set; }
        public BooleanMode BooleanMode { get; set; }
        public bool IsSubject { get; set; }

        internal ValueInfo ValueInfo { get; set; }

        internal void SetValue(CommandOption commandOption, List<string> remainingArguments)
        {
            this.ValueInfo = new ValueInfo(commandOption, remainingArguments);
        }

        private BooleanMode GetBooleanMode()
        {
            ArgumentAttribute attribute = _parameterInfo.GetCustomAttribute<ArgumentAttribute>();

            if (attribute == null || attribute.BooleanMode == BooleanMode.Unknown)
                return _settings.BooleanMode;

            if (Type == typeof(bool) || Type == typeof(bool?))
            {
                return attribute.BooleanMode;
            }

            throw new AppRunnerException(
                $"BooleanMode property is set to `{attribute.BooleanMode}` for a non boolean parameter type. " +
                $"Property name: {_parameterInfo.Name} " +
                $"Type : {Type.Name}");
        }

        private bool GetIsSubject()
        {
            return _parameterInfo.GetCustomAttribute<SubjectAttribute>() != null;
        }
        
        private bool GetIsParameterRequired()
        {
            if (BooleanMode == BooleanMode.Implicit && (Type == typeof(bool) || Type == typeof(bool?))) return false;

            ArgumentAttribute descriptionAttribute = _parameterInfo.GetCustomAttribute<ArgumentAttribute>(false);

            if (descriptionAttribute != null && Type == typeof(string))
            {
                if (_parameterInfo.HasDefaultValue & descriptionAttribute.RequiredString)
                    throw new AppRunnerException(
                        $"String parameter '{Name}' can't be 'Required' and have a default value at the same time");

                return descriptionAttribute.RequiredString;
            }

            if (descriptionAttribute != null && Type != typeof(string) && descriptionAttribute.RequiredString)
            {
                throw new AppRunnerException("RequiredString can only me used with a string type parameter");
            }

            return _parameterInfo.ParameterType.IsValueType
                   && _parameterInfo.ParameterType.IsPrimitive
                   && !_parameterInfo.HasDefaultValue;
        }

        private string GetAnnotatedDescription()
        {
            ArgumentAttribute descriptionAttribute = _parameterInfo.GetCustomAttribute<ArgumentAttribute>();
            return descriptionAttribute?.Description;
        }

        private string GetTypeDisplayName()
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

        private string GetDetails()
        {
            return $"{this.GetTypeDisplayName()}{(this.Required ? " | Required" : null)}" +
                   $"{(this.DefaultValue != DBNull.Value ? " | Default value: " + DefaultValue : null)}";
        }

        private string GetEffectiveDescription()
        {
            return _settings.ShowParameterDetails
                ? string.Format("{0}{1}", Details.PadRight(Constants.PadLength), AnnotatedDescription)
                : AnnotatedDescription;
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

        private string GetTemplate(ParameterInfo parameterInfo)
        {
            ArgumentAttribute attribute = parameterInfo.GetCustomAttribute<ArgumentAttribute>(false);

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

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case ArgumentInfo argumentInfo:
                    return argumentInfo.Template == this.Template;
                case CommandOption commandOption:
                    return commandOption.Template == this.Template;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Template.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Name} | '{ValueInfo?.Value ?? "null"}' | {Details} | {Template}";
        }
    }
    
    internal class ValueInfo
    {
        private readonly CommandOption _commandOption;
        private readonly List<string> _remainingArguments;

        public ValueInfo(CommandOption commandOption, List<string> remainingArguments)
        {
            _commandOption = commandOption;
            _remainingArguments = remainingArguments = remainingArguments ?? new List<string>();
        }

        internal bool HasValue => _remainingArguments.Any() || _commandOption.HasValue();

        internal List<string> Values => _remainingArguments.Any() ? _remainingArguments : _commandOption.Values;

        internal string Value => _remainingArguments.Any() ? _remainingArguments.First() : _commandOption.Value();

        public override string ToString()
        {
            return _remainingArguments.Any() ? _remainingArguments.First() : _commandOption?.Value();
        }
    } 
}