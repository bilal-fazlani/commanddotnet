using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.ClassModeling.Definitions;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using CommandDotNet.Parsing;

namespace CommandDotNet.ClassModeling
{
    internal static class BindValuesMiddleware
    {
        internal static Task<int> BindValues(CommandContext commandContext, ExecutionDelegate next)
        {
            var console = commandContext.Console;
            var parserFactory = new ParserFactory(commandContext.AppConfig.AppSettings);

            bool SetFromStringInput(IArgumentDef argDef, IEnumerable<string> values, out int returnCode)
            {
                try
                {
                    var argument = argDef.Argument;
                    var parser = parserFactory.CreateInstance(argument);
                    var value = parser.Parse(argument, values);
                    argDef.SetValue(value);
                    returnCode = 0;
                    return true;
                }
                catch (ValueParsingException ex)
                {
                    console.Error.WriteLine($"Failure parsing value for {argDef}.  values={values?.ToCsv()}");
                    console.Error.WriteLine(ex.Message);
                    console.Error.WriteLine();
                    returnCode = 2;
                    return false;
                }
            }

            var argumentDefs = commandContext.InvocationPipeline.All
                .SelectMany(ic => ic.Command.Options.Cast<IArgument>().Union(ic.Command.Operands))
                .Select(a => a.GetArgumentDef())
                .Where(d => d != null);

            foreach (var argumentDef in argumentDefs)
            {
                var argument = argumentDef.Argument;
                if (!argument.RawValues.IsNullOrEmpty())
                {
                    if (!SetFromStringInput(argumentDef, argument.RawValues, out int returnCode))
                    {
                        return Task.FromResult(returnCode);
                    }
                }
                else if (argument.DefaultValue != null)
                {
                    var defaultValue = argument.DefaultValue;
                    // middleware could set the default to any type.
                    // string values could be assigned from attributes or config values
                    // so they are parsed as if submitted from the shell.
                    switch (defaultValue)
                    {
                        case string stringValue:
                            if (!SetFromStringInput(argumentDef, stringValue.ToEnumerable(), out int returnCode))
                            {
                                return Task.FromResult(returnCode);
                            }

                            break;
                        case IEnumerable<string> multiString:
                            if (!SetFromStringInput(argumentDef, multiString, out returnCode))
                            {
                                return Task.FromResult(returnCode);
                            }

                            break;
                        default:
                            try
                            {
                                if (argumentDef.Type.IsInstanceOfType(defaultValue))
                                {
                                    argumentDef.SetValue(defaultValue);
                                }
                                else
                                {
                                    // cover cases like converting int to long.
                                    var convertedValue = Convert.ChangeType(defaultValue, argumentDef.Type);
                                    argumentDef.SetValue(convertedValue);
                                }
                            }
                            catch (Exception ex)
                            {
                                console.Error.WriteLine(
                                    $"Failure assigning value to {argumentDef}. Value={defaultValue}");
                                console.Error.WriteLine(ex.Message);
                                console.Error.WriteLine();
                                return Task.FromResult(2);
                            }

                            break;
                    }
                }
            }

            return next(commandContext);
        }
    }
}