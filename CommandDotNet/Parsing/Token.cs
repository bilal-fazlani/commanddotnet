namespace CommandDotNet.Parsing
{
    public class Token
    {
        public string RawValue { get; }
        public string Value { get; }
        public TokenType TokenType { get; }
        public OptionTokenType OptionTokenType { get; }

        public Token(
            string rawValue,
            string value, 
            TokenType tokenType, 
            OptionTokenType optionTokenType = null)
        {
            RawValue = rawValue;
            Value = value;
            TokenType = tokenType;
            OptionTokenType = optionTokenType;
        }

        public override string ToString()
        {
            return $"{nameof(Token)}:{TokenType}>{RawValue}";
        }
    }
}