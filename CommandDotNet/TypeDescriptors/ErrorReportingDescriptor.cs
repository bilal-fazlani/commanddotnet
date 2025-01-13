using System;
using System.Collections.Generic;
using CommandDotNet.Parsing;

namespace CommandDotNet.TypeDescriptors;

internal class ErrorReportingDescriptor(IArgumentTypeDescriptor innerDescriptor)
    : IArgumentTypeDescriptor, IAllowedValuesTypeDescriptor
{
    public bool CanSupport(Type type) => innerDescriptor.CanSupport(type);

    public string GetDisplayName(IArgument argument) => innerDescriptor.GetDisplayName(argument);

    public object? ParseString(IArgument argument, string value)
    {
        try
        {
            return innerDescriptor.ParseString(argument, value);
        }
        catch (FormatException)
        {
            throw new ValueParsingException(
                Resources.A.Error_Value_is_not_valid_for_type(value, 
                    innerDescriptor.GetDisplayName(argument)));
        }
        catch (ArgumentException)
        {
            throw new ValueParsingException(
                Resources.A.Error_Value_is_not_valid_for_type(value, 
                    innerDescriptor.GetDisplayName(argument)));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ValueParsingException(
                Resources.A.Error_Failed_parsing_value_for_type(value, 
                    innerDescriptor.GetDisplayName(argument)), ex);
        }
    }

    public IEnumerable<string> GetAllowedValues(IArgument argument) =>
        (innerDescriptor as IAllowedValuesTypeDescriptor)?.GetAllowedValues(argument)
        ?? [];

    public override string ToString() => $"{nameof(ErrorReportingDescriptor)} > {innerDescriptor}";
}