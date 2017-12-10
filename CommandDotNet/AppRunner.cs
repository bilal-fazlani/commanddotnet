using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CommandDotNet.Attributes;
using CommandDotNet.Exceptions;
using CommandDotNet.Models;
using Microsoft.Extensions.CommandLineUtils;

[assembly: InternalsVisibleTo("CommandDotNet.Tests")]

namespace CommandDotNet
{
    /// <summary>
    /// Creates a new instance of AppRunner
    /// </summary>
    /// <typeparam name="T">Type of the application</typeparam>
    public class AppRunner<T> where T : class
    {
        internal readonly CommandLineApplication App = new CommandLineApplication();

        private readonly AppSettings _settings;

        private readonly bool _error = false;

        public AppRunner(AppSettings settings = null)
        {
            try
            {
                _settings = settings ?? new AppSettings();

                App.HelpOption(Constants.HelpTemplate);

                ApplicationMetadataAttribute consoleApplicationAttribute =
                    typeof(T).GetCustomAttribute<ApplicationMetadataAttribute>(false);
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
                    optionValues.Add(optionInfo,
                        App.Option(optionInfo.Template, optionInfo.EffectiveDescription, optionInfo.CommandOptionType));
                }

                IEnumerable<CommandInfo> commands = typeof(T)
                    .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Where(m => !m.IsSpecialName)
                    .Where(m => m.GetCustomAttribute<DefaultMethodAttribute>() == null)
                    .Select(mi => new CommandInfo(mi, _settings));


                CommandInfo defaultCommand = typeof(T)
                    .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Where(m => !m.IsSpecialName)
                    .Where(m => m.GetCustomAttribute<DefaultMethodAttribute>() != null)
                    .Select(mi => new CommandInfo(mi, _settings))
                    .FirstOrDefault();

                Dictionary<ArgumentInfo, CommandOption> defaultCommandParameterValues =
                    new Dictionary<ArgumentInfo, CommandOption>();

                App.OnExecute(() =>
                {
                    if (defaultCommand != null)
                    {
                        if (defaultCommand.Parameters.Any())
                        {
                            throw new Exception("Method with [DefaultMethod] attribute does not support parameters");
                        }

                        try
                        {
                            T instance = AppFactory.CreateApp<T>(optionValues);

                            MethodInfo theMethod = typeof(T).GetMethod(defaultCommand.MethodName);

                            object returnedObject = theMethod.Invoke(instance,
                                defaultCommandParameterValues.Select(ValueMachine.GetValue).ToArray());

                            return (int) (returnedObject ?? 0);
                        }
                        catch (ValueParsingException e)
                        {
                            throw new CommandParsingException(App, e.Message);
                        }
                    }

                    App.ShowHelp();
                    return 0;
                });

                foreach (CommandInfo commandInfo in commands)
                {
                    Dictionary<ArgumentInfo, CommandOption> parameterValues =
                        new Dictionary<ArgumentInfo, CommandOption>();

                    var commandOption = App.Command(commandInfo.Name, command =>
                    {
                        command.Description = commandInfo.Description;

                        command.ExtendedHelpText = commandInfo.ExtendedHelpText;

                        command.HelpOption(Constants.HelpTemplate);

                        foreach (ArgumentInfo parameter in commandInfo.Parameters)
                        {
                            parameterValues.Add(parameter, command.Option(parameter.Template,
                                parameter.EffectiveDescription,
                                parameter.CommandOptionType));
                        }
                    });

                    commandOption.OnExecute(() =>
                    {
                        try
                        {
                            T instance = AppFactory.CreateApp<T>(optionValues);

                            MethodInfo theMethod = typeof(T).GetMethod(commandInfo.MethodName);

                            object returnedObject = theMethod.Invoke(instance,
                                parameterValues.Select(ValueMachine.GetValue).ToArray());

                            return (int) (returnedObject ?? 0);
                        }
                        catch (ValueParsingException e)
                        {
                            throw new CommandParsingException(App, e.Message);
                        }
                    });
                }
            }
            catch (AppRunnerException e)
            {
                _error = true;
                Console.Error.WriteLine(e.Message + "\n");
#if DEBUG
                Console.Error.WriteLine(e.StackTrace);
#endif
            }
        }

        /// <summary>
        /// Exceutes the specified command with given parameters
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns>If target method returns int, this method will return that value. Else, 
        /// it will return 0 in case of successs and 1 in case of unhandled exception</returns>
        public int Run(string[] args)
        {
            if (_error) return 1;

            try
            {
                return App.Execute(args);
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