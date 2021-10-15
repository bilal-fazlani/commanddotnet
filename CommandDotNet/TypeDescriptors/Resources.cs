// ReSharper disable CheckNamespace

using System;
using System.ComponentModel;
using CommandDotNet.TypeDescriptors;

namespace CommandDotNet
{
    public partial class Resources
    {
        public virtual string Type_Boolean => "Boolean";
        public virtual string Type_Character => "Character";
        public virtual string Type_Decimal => "Decimal";
        public virtual string Type_Double => "Double";
        public virtual string Type_Number => "Number";
        public virtual string Type_Text => "Text";

        public virtual string Error_Value_is_not_valid_for_type(string value, string typeDisplayName)
            => $"'{value}' is not a valid {typeDisplayName}";

        public virtual string Error_Failed_parsing_value_for_type(string value, string typeDisplayName)
            => $"failed parsing '{value}' for {typeDisplayName}";

        public string Error_Type_is_not_supported_as_argument(Type type)
            => $"type : {type} is not supported. " +
               Environment.NewLine +
               $"If it is an argument model, inherit from {nameof(IArgumentModel)}. " +
               Environment.NewLine +
               "If it is a service and not an argument, register using " +
               $"{nameof(AppRunner)}.{nameof(AppRunner.Configure)}(b => b.{nameof(AppConfigBuilder.UseParameterResolver)}(ctx => ...)); " +
               Environment.NewLine +
               "Otherwise, to support this type, " +
               $"implement a {nameof(TypeConverter)} or {nameof(IArgumentTypeDescriptor)} " +
               "or add a constructor with a single string parameter.";
    }
}