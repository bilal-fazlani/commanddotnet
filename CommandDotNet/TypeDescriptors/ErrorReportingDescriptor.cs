using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.ClassModeling;
using CommandDotNet.Parsing;

namespace CommandDotNet.TypeDescriptors
{
    internal class ErrorReportingDescriptor: IArgumentTypeDescriptor, IAllowedValuesTypeDescriptor
    {
        private readonly IArgumentTypeDescriptor _innerDescriptor;

        public ErrorReportingDescriptor(IArgumentTypeDescriptor innerDescriptor)
        {
            _innerDescriptor = innerDescriptor;
        }

        public bool CanSupport(Type type)
        {
            return _innerDescriptor.CanSupport(type);
        }

        public string GetDisplayName(ArgumentInfo argumentInfo)
        {
            return _innerDescriptor.GetDisplayName(argumentInfo);
        }

        public object ParseString(ArgumentInfo argumentInfo, string value)
        {
            try
            {
                return _innerDescriptor.ParseString(argumentInfo, value);
            }
            catch (FormatException)
            {
                throw new ValueParsingException(
                    $"'{value}' is not a valid {_innerDescriptor.GetDisplayName(argumentInfo)}");
            }
            catch (ArgumentException)
            {
                throw new ValueParsingException(
                    $"'{value}' is not a valid {_innerDescriptor.GetDisplayName(argumentInfo)}");
            }
        }

        public IEnumerable<string> GetAllowedValues(ArgumentInfo argumentInfo)
        {
            return (_innerDescriptor as IAllowedValuesTypeDescriptor)?.GetAllowedValues(argumentInfo) 
                   ?? Enumerable.Empty<string>();
        }
    }
}