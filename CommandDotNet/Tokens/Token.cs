namespace CommandDotNet.Tokens
{
    public class Token
    {
        /// <summary>
        /// The raw value from the user input.
        /// This will contain the punctuation used to denote option and argument names.
        /// </summary>
        public string RawValue { get; }

        /// <summary>Can be an Option name or an argument value</summary>
        public string Value { get; }

        /// <summary>The <see cref="Tokens.TokenType"/></summary>
        public TokenType TokenType { get; }

        public string? SourceName { get; internal set; }

        public Token? SourceToken { get; internal set; }

        public Token(
            string rawValue,
            string value, 
            TokenType tokenType)
        {
            RawValue = rawValue;
            Value = value;
            TokenType = tokenType;
        }

        public override string ToString()
        {
            return $"{nameof(Token)}:{TokenType}>{RawValue}";
        }
    }
}