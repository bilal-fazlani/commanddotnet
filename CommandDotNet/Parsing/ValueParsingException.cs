using System;

namespace CommandDotNet.Parsing
{
    public class ValueParsingException : Exception
    {
        public ValueParsingException(string message) : base(message)
        {
            
        }
    }
}