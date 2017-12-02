using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.Attributes;
using CommandDotNet.Exceptions;
using CommandDotNet.Models;
using Microsoft.Extensions.CommandLineUtils;

namespace CommandDotNet
{
    public class AppRunner<T> where T: class
    {        
        private readonly CommandLineApplication _app = new CommandLineApplication();

        private readonly AppSettings _settings;
        
        public AppRunner(AppSettings settings = null)
        {
            _settings = settings ?? new AppSettings();
            
            _app.HelpOption(Constants.HelpTemplate);

            ApplicationMetadataAttribute consoleApplicationAttribute = typeof(T).GetCustomAttribute<ApplicationMetadataAttribute>(false);
            _app.Name = $"dotnet {Assembly.GetCallingAssembly().GetName().Name}.dll";
            
            _app.FullName = consoleApplicationAttribute?.Description;
            
            _app.ExtendedHelpText = consoleApplicationAttribute?.ExtendedHelpText;

            IEnumerable<ArguementInfo> options = typeof(T)
                .GetConstructors()
                .SingleOrDefault()
                .GetParameters()
                .Select(p => new ArguementInfo(p, _settings));
            
            Dictionary<ArguementInfo, CommandOption> optionValues = new Dictionary<ArguementInfo, CommandOption>();
            
            foreach (ArguementInfo optionInfo in options)
            {
                optionValues.Add(optionInfo, _app.Option(optionInfo.Template, optionInfo.EffectiveDescription, optionInfo.CommandOptionType));
            }
            
            IEnumerable<CommandInfo> commands = typeof(T)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsSpecialName)
                .Select(mi => new CommandInfo(mi, _settings));

            foreach (CommandInfo commandInfo in commands)
            {
                Dictionary<ArguementInfo, CommandOption> parameterValues = new Dictionary<ArguementInfo, CommandOption>();
                
                var commandOption = _app.Command(commandInfo.Name, command =>
                {
                    command.Description = commandInfo.Description;

                    command.ExtendedHelpText = commandInfo.ExtendedHelpText;
                    
                    command.HelpOption(Constants.HelpTemplate);

                    foreach (ArguementInfo parameter in commandInfo.Parameters)
                    {
                        parameterValues.Add(parameter, command.Option(parameter.Template, parameter.EffectiveDescription,
                            parameter.CommandOptionType));
                    }
                });
                
                commandOption.OnExecute(() =>
                {
                    try
                    {
                        T instance = AppFactory.CreateApp<T>(optionValues);
                    
                        MethodInfo theMethod = typeof(T).GetMethod(commandInfo.MethodName);
                    
                        theMethod.Invoke(instance, parameterValues.Select(ValueMachine.GetValue).ToArray());
                        return 0;
                    }
                    catch (ValueParsingException e)
                    {
                        throw new CommandParsingException(_app, e.Message);
                    }
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
                Console.Error.WriteLine(e.Message + "\n");
                _app.ShowHelp();
                return 1;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message + "\n");
#if DEBUG
         Console.Error.WriteLine(e.StackTrace);       
#endif
                return 1;
            }
        }
    }
}