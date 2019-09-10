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
            var commandDef = commandContext.ParseResult.TargetCommand.Services.Get<ICommandDef>();
            if (commandDef != null)
            {
                var console = commandContext.Console;
                var argumentValues = commandContext.ParseResult.ArgumentValues;
                var parserFactory = new ParserFactory(commandContext.AppConfig.AppSettings);

                var interceptorArgs = commandDef.InterceptorMethodDef.ArgumentDefs;
                var invokeArgs = commandDef.InvokeMethodDef.ArgumentDefs;

                bool SetFromStringInput(IArgumentDef argDef, IEnumerable<string> values, out int returnCode)
                {
                    try
                    {
                        var parser = parserFactory.CreateInstance(argDef.Argument);
                        var value = parser.Parse(argDef.Argument, values);
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

                foreach (var argumentDef in interceptorArgs.Union(invokeArgs))
                {
                    if (argumentValues.TryGetValues(argumentDef.Argument, out var values))
                    {
                        if (!SetFromStringInput(argumentDef, values, out int returnCode))
                        {
                            return Task.FromResult(returnCode);
                        }
                    }
                    else if (argumentDef.Argument.DefaultValue != null)
                    {
                        var defaultValue = argumentDef.Argument.DefaultValue;
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
                                    console.Error.WriteLine($"Failure assigning value to {argumentDef}. Value={defaultValue}");
                                    console.Error.WriteLine(ex.Message);
                                    console.Error.WriteLine();
                                    return Task.FromResult(2);
                                }
                                break;
                        }
                    }
                }
            }
            return next(commandContext);
        }
    }
}