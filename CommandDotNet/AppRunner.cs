using System;
using System.Reflection;
using System.Runtime.CompilerServices;
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
        private AppSettings _settings;

        public AppRunner(AppSettings settings = null)
        {
            _settings = settings ?? new AppSettings();
        }

        /// <summary>
        /// Exceutes the specified command with given parameters
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns>If target method returns int, this method will return that value. Else, 
        /// it will return 0 in case of successs and 1 in case of unhandled exception</returns>
        public int Run(string[] args)
        {
            try
            {                
                string name = $"dotnet {Assembly.GetCallingAssembly().GetName().Name}.dll";
                
                CommandCreator commandCreator = new CommandCreator(typeof(T));

                CommandLineApplication app = commandCreator.CreateCommand(_settings, name);
                                
                return app.Execute(args);
            }
            catch (AppRunnerException e)
            {
                Console.Error.WriteLine(e.Message + "\n");
#if DEBUG
                Console.Error.WriteLine(e.StackTrace);
#endif
                return 1;
            }
            catch (CommandParsingException e)
            {
                Console.Error.WriteLine(e.Message + "\n");
                e.Command.ShowHelp();

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