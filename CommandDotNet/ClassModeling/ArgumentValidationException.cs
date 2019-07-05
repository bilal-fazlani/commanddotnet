using System;
using FluentValidation.Results;

namespace CommandDotNet.ClassModeling
{
    public class ArgumentValidationException : Exception
    {
        public ValidationResult ValidationResult { get; }

        public ArgumentValidationException(ValidationResult validationResult)
        {
            ValidationResult = validationResult;
        }
    }
}