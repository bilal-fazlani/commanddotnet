using System;

namespace CommandDotNet
{
    /// <summary>The values provided as the default for an argument</summary>
    public class ArgumentDefault
    {
        /// <summary>The source of the default value</summary>
        public string Source { get; }

        /// <summary>The key of the default value</summary>
        public string Key { get; }

        /// <summary>The text values</summary>
        public object Value { get; }

        public ArgumentDefault(string source, string key, object value)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public override string ToString()
        {
            // do not include value in case it's a password
            return $"{nameof(ArgumentDefault)}: {Source}.{Key}";
        }
    }
}