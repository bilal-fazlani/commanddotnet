using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.MicrosoftCommandLineUtils;

namespace CommandDotNet.Models
{
    public abstract class ArgumentInfo
    {
        protected readonly ParameterInfo ParameterInfo;
        protected readonly AppSettings Settings;

        internal ArgumentInfo(AppSettings settings)
        {
            Settings = settings;
        }

        internal ArgumentInfo(ParameterInfo parameterInfo, AppSettings settings)
            : this(settings)
        {
            ParameterInfo = parameterInfo;

            Name = parameterInfo.Name;
            Type = parameterInfo.ParameterType;
            
            TypeDisplayName = GetTypeDisplayName();
            AnnotatedDescription = GetAnnotatedDescription();
            DefaultValue = parameterInfo.DefaultValue;
            Details = GetDetails();
            EffectiveDescription = GetEffectiveDescription();
            IsMultipleType = GetIsMultipleType();
        }

        public string Name { get; set; }
        public Type Type { get; set; }
        
        public object DefaultValue { get; set; }
        public string TypeDisplayName { get; set; }
        public string Details { get; set; }
        public string AnnotatedDescription { get; set; }
        public string EffectiveDescription { get; set; }
        public bool IsMultipleType { get; }
        internal ValueInfo ValueInfo { get; set; }

        private bool GetIsMultipleType()
        {
            return Type != typeof(string) && Type.IsCollection();
        }
        
        internal void SetValue(IParameter parameter)
        {
            ValueInfo = new ValueInfo(parameter);
        }

        protected abstract string GetAnnotatedDescription();

        protected abstract string GetTypeDisplayName();
        
        protected abstract string GetDetails();

        
        private string GetEffectiveDescription()
        {
            return Settings.ShowArgumentDetails
                ? $"{Details.PadRight(Constants.PadLength)}{AnnotatedDescription}"
                : AnnotatedDescription;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}