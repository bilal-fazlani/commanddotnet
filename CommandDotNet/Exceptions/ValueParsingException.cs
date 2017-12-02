using System;

namespace CommandDotNet.Exceptions
{
    public class ValueParsingException : Exception
    {
        public ValueParsingException(string message) : base(message)
        {
            
        }
    }
}