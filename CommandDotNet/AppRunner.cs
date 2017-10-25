using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.CommandLineUtils;

namespace CommandDotNet
{
    public class AppRunner<T> where T : new()
    {
        private readonly T _instance = new T();
        private readonly CommandLineApplication _app = new CommandLineApplication();
        
        public AppRunner()
        {
            _app.HelpOption("-h | -? | --help");
            
            
            var methods = typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(mi => new CommandInfo(mi));

            foreach (var method in methods)
            {
                Dictionary<string, CommandOption> parameterValues = new Dictionary<string, CommandOption>();
                
                var commandOption = _app.Command(method.MethodName, command =>
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
                int result = _app.Execute(args);
                return result;
            }
            catch (CommandParsingException e)
            {
                Console.WriteLine(e.Message);
                _app.ShowHelp();
                return 1;
            }
        }
    }
}