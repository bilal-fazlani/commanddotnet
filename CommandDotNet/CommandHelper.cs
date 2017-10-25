using System;
using System.Collections;
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
            app.HelpOption("-h | -? | --help");
            
            var getCommandOptionType = new Func<ParameterInfo, CommandOptionType>((ParameterInfo pi) =>
            {
                if (pi.ParameterType.IsAssignableFrom(typeof(IEnumerable)))
                {
                    return CommandOptionType.MultipleValue;
                }
                
                if(pi.ParameterType.IsAssignableFrom(typeof(bool)))
                {
                    return CommandOptionType.NoValue;
                }

                return CommandOptionType.SingleValue;
            });
            
            var methods = typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(mi => !(new [] {"Equals", "GetHashCode", "GetType", "ToString"}.Contains(mi.Name)))
                .Select(mi => new
                {
                    MethodName = mi.Name,
                    Parameters = mi.GetParameters().Select(pi => new
                    {
                        ParameterName = pi.Name,
                        ParameterType = pi.ParameterType,
                        CommandOptionType = getCommandOptionType(pi)
                    })
                });

            foreach (var method in methods)
            {
                Dictionary<string, CommandOption> parameterValues = new Dictionary<string, CommandOption>();
                
                var commandOption = app.Command(method.MethodName, command =>
                {
                    command.Description = "command description";
                    
                    command.HelpOption("-h | -? | --help");

                    foreach (var parameter in method.Parameters)
                    {
                        parameterValues.Add(parameter.ParameterName, command.Option($"--{parameter.ParameterName}", "parameter description",
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