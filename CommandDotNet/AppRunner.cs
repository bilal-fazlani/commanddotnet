using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CommandDotNet.Attributes;
using CommandDotNet.Exceptions;
using CommandDotNet.Models;
using Microsoft.Extensions.CommandLineUtils;

[assembly:InternalsVisibleTo("CommandDotNet.Tests")]

namespace CommandDotNet
{
    public class AppRunner<T> where T: class
    {        
        internal readonly CommandLineApplication App = new CommandLineApplication();

        private readonly AppSettings _settings;
        
        public AppRunner(AppSettings settings = null)
        {
            _settings = settings ?? new AppSettings();
            
            App.HelpOption(Constants.HelpTemplate);

            ApplicationMetadataAttribute consoleApplicationAttribute = typeof(T).GetCustomAttribute<ApplicationMetadataAttribute>(false);
            App.Name = $"dotnet {Assembly.GetCallingAssembly().GetName().Name}.dll";
            
            App.FullName = consoleApplicationAttribute?.Description;
            
            App.ExtendedHelpText = consoleApplicationAttribute?.ExtendedHelpText;
            
            IEnumerable<ArgumentInfo> options = typeof(T)
                .GetConstructors()
                .FirstOrDefault()
                .GetParameters()
                .Select(p => new ArgumentInfo(p, _settings));
            
            Dictionary<ArgumentInfo, CommandOption> optionValues = new Dictionary<ArgumentInfo, CommandOption>();
            
            foreach (ArgumentInfo optionInfo in options)
            {
                optionValues.Add(optionInfo, App.Option(optionInfo.Template, optionInfo.EffectiveDescription, optionInfo.CommandOptionType));
            }
            
            IEnumerable<CommandInfo> commands = typeof(T)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsSpecialName)
                .Select(mi => new CommandInfo(mi, _settings));

            foreach (CommandInfo commandInfo in commands)
            {
                Dictionary<ArgumentInfo, CommandOption> parameterValues = new Dictionary<ArgumentInfo, CommandOption>();
                
                var commandOption = App.Command(commandInfo.Name, command =>
                {
                    command.Description = commandInfo.Description;

                    command.ExtendedHelpText = commandInfo.ExtendedHelpText;
                    
                    command.HelpOption(Constants.HelpTemplate);

                    foreach (ArgumentInfo parameter in commandInfo.Parameters)
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
                        throw new CommandParsingException(App, e.Message);
                    }
                });
            }
        }

        public int Run(string[] args)
        {
            try
            {
                int result = App.Execute(args);
                return result;
            }
            catch (CommandParsingException e)
            {
                Console.Error.WriteLine(e.Message + "\n");
                App.ShowHelp();
                
#if DEBUG
                Console.Error.WriteLine(e.StackTrace);       
#endif
                
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