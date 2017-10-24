using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.CommandLineUtils;

namespace CommandDotNet
{
    public class CommandHelper<T> where T:new()
    {
        private readonly T _instance = new T();
        CommandLineApplication app = new CommandLineApplication();
        
        public CommandHelper()
        {
            var methods = new[]
            {
                new
                {
                    MethodName = "Paint",
                    Parameters = new[]
                    {
                        new
                        {
                            ParameterName = "color",
                            //ParameterType = "string",
                            CommandOptionType = CommandOptionType.SingleValue
                        }
                    }
                }
            };

            foreach (var method in methods)
            {
                
                Dictionary<string, CommandOption> parameterValues = new Dictionary<string, CommandOption>();
                
                var commandOption = app.Command(method.MethodName, command =>
                {
                    command.Description = "sample description";

                    foreach (var parameter in method.Parameters)
                    {
                        parameterValues.Add(parameter.ParameterName, command.Option($"--{parameter.ParameterName}", "sample description",
                            parameter.CommandOptionType));
                    }
                });
                
                commandOption.OnExecute(() =>
                {
                    MethodInfo theMethod = typeof(T).GetMethod(method.MethodName);
                    theMethod.Invoke(_instance, parameterValues.Select(x => x.Value.Value()).ToArray());
                    return 0;
                });
            }
        }
        
        public int Run(string[] args)
        {
            try
            {
                int result = app.Execute(args);
                return result;
            }
            catch (CommandParsingException e)
            {
                Console.WriteLine(e.Message);
                app.ShowHelp();
                return 1;
            }
        }
    }
}