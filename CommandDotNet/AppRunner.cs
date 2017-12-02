using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.Attributes;
using CommandDotNet.Models;
using Microsoft.Extensions.CommandLineUtils;

namespace CommandDotNet
{
    public class AppRunner<T> where T : new()
    {
        private readonly T _instance = new T();
        private readonly CommandLineApplication _app = new CommandLineApplication();

        private readonly AppSettings _settings;
        
        public AppRunner(AppSettings settings = null)
        {
            _settings = settings ?? new AppSettings();
            
            _app.HelpOption("-h | -? | --help");

            ConsoleApplicationAttribute consoleApplicationAttribute = typeof(T).GetCustomAttribute<ConsoleApplicationAttribute>(false);
            _app.Name = $"dotnet {Assembly.GetCallingAssembly().GetName().Name}.dll";
            _app.FullName = consoleApplicationAttribute?.Description;
            
            var commands = typeof(T)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsSpecialName)
                .Select(mi => new CommandInfo(mi, _settings));

            foreach (var method in commands)
            {
                Dictionary<string, CommandOption> parameterValues = new Dictionary<string, CommandOption>();
                
                var commandOption = _app.Command(method.MethodName, command =>
                {
                    command.Description = method.Description;
                    
                    command.HelpOption("-h | -? | --help");

                    foreach (var parameter in method.Parameters)
                    {
                        parameterValues.Add(parameter.ParameterName, command.Option($"--{parameter.ParameterName}", parameter.Description,
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