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
            
            _app.HelpOption(Constants.HelpTemplate);

            ApplicationMetadataAttribute consoleApplicationAttribute = typeof(T).GetCustomAttribute<ApplicationMetadataAttribute>(false);
            _app.Name = $"dotnet {Assembly.GetCallingAssembly().GetName().Name}.dll";
            _app.FullName = consoleApplicationAttribute?.Description;

            IEnumerable<ArguementInfo> options = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(p => new ArguementInfo(p, _settings));

            foreach (ArguementInfo optionInfo in options)
            {
                _app.Option(optionInfo.Template, optionInfo.Description, optionInfo.CommandOptionType);
            }
            
            IEnumerable<CommandInfo> commands = typeof(T)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsSpecialName)
                .Select(mi => new CommandInfo(mi, _settings));

            foreach (CommandInfo commandInfo in commands)
            {
                Dictionary<string, CommandOption> parameterValues = new Dictionary<string, CommandOption>();
                
                var commandOption = _app.Command(commandInfo.Name, command =>
                {
                    command.Description = commandInfo.Description;
                    
                    command.HelpOption(Constants.HelpTemplate);

                    foreach (var parameter in commandInfo.Parameters)
                    {
                        parameterValues.Add(parameter.Name, command.Option(parameter.Template, parameter.Description,
                            parameter.CommandOptionType));
                    }
                });
                
                commandOption.OnExecute(() =>
                {
                    MethodInfo theMethod = typeof(T).GetMethod(commandInfo.Name);
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