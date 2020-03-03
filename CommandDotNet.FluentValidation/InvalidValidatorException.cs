using System;

namespace CommandDotNet.FluentValidation
{
    internal class InvalidValidatorException : Exception
    {
        public InvalidValidatorException(string message, Exception exception) : base(message, exception)
        { }
    }
}