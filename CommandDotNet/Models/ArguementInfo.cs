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
            Type = parameterInfo.ParameterType;
            DefaultValue = parameterInfo.DefaultValue;
            IsMultipleType = GetIsMultipleType();
        }

        
        public Type Type { get; internal set; }
        
        public object DefaultValue { get; internal set; }
        public string TypeDisplayName { get; internal set; }
        public string Details { get; internal set; }
        public string AnnotatedDescription { get; internal set; }
        public string EffectiveDescription { get; internal set; }
        public bool IsMultipleType { get; }
        internal ValueInfo ValueInfo { get; private set; }

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
        
        protected string GetEffectiveDescription()
        {
            return Settings.ShowArgumentDetails
                ? $"{Details.PadRight(Constants.PadLength)}{AnnotatedDescription}"
                : AnnotatedDescription;
        }
    }
}