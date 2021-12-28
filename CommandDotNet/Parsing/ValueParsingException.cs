using System;

namespace CommandDotNet.Parsing
{
    /// <summary><see cref="ValueParsingException"/> indicates user error.</summary>
    public class ValueParsingException : Exception
    {
        public ValueParsingException(string message) : base(message)
        {

        }

        public ValueParsingException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}