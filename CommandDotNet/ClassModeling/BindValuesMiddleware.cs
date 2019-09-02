using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.ClassModeling.Definitions;
using CommandDotNet.Extensions;
using CommandDotNet.Parsing;

namespace CommandDotNet.ClassModeling
{
    internal static class BindValuesMiddleware
    {
        internal static Task<int> BindValues(CommandContext commandContext, Func<CommandContext, Task<int>> next)
        {
            var commandDef = commandContext.ParseResult.TargetCommand.Services.Get<ICommandDef>();
            if (commandDef != null)
            {
                var console = commandContext.Console;
                var argumentValues = commandContext.ParseResult.ArgumentValues;
                var parserFactory = new ParserFactory(commandContext.AppConfig.AppSettings);

                var interceptorArgs = commandDef.InterceptorMethodDef.ArgumentDefs;
                var invokeArgs = commandDef.InvokeMethodDef.ArgumentDefs;

                void SetFromStringInput(IArgumentDef argDef, List<string> values)
                {
                    var parser = parserFactory.CreateInstance(argDef.Argument);
                    var value = parser.Parse(argDef.Argument, values);
                    argDef.SetValue(value);
                }

                foreach (var argumentDef in interceptorArgs.Union(invokeArgs))
                {
                    List<string> values = null;
                    try
                    {
                        if (argumentValues.TryGetValues(argumentDef.Argument, out values))
                        {
                            SetFromStringInput(argumentDef, values);
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
                                    SetFromStringInput(argumentDef, values = new List<string> { stringValue });
                                    break;
                                case IEnumerable<string> multiString:
                                    SetFromStringInput(argumentDef, values = multiString.ToList());
                                    break;
                                default:
                                    if (argumentDef.Type.IsInstanceOfType(defaultValue))
                                    {
                                        argumentDef.SetValue(defaultValue);
                                    }
                                    else
                                    {
                                        try
                                        {
                                            // cover cases like converting int to long.
                                            var convertedValue = Convert.ChangeType(defaultValue, argumentDef.Type);
                                            argumentDef.SetValue(convertedValue);
                                        }
                                        catch (Exception ex)
                                        {
                                            console.Error.WriteLine($"Failure assigning value to {argumentDef}.");
                                            console.Error.WriteLine(ex.Message);
                                            console.Error.WriteLine();
                                            return Task.FromResult(2);
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                    catch (ValueParsingException ex)
                    {
                        console.Error.WriteLine($"Failure parsing value for {argumentDef}.  values={values?.ToCsv()}");
                        console.Error.WriteLine(ex.Message);
                        console.Error.WriteLine();
                        return Task.FromResult(2);
                    }
                }
            }
            return next(commandContext);
        }
    }
}