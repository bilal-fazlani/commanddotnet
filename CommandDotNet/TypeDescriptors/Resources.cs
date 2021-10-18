// ReSharper disable CheckNamespace

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
    }
}