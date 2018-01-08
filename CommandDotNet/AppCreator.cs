using System;
using System.Reflection;
using CommandDotNet.Attributes;
using CommandDotNet.MicrosoftCommandLineUtils;
using CommandDotNet.Models;

namespace CommandDotNet
{
    public class AppCreator
    {
        private readonly AppSettings _appSettings;

        public AppCreator(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public CommandLineApplication CreateApplication(
            Type type,
            IDependencyResolver dependencyResolver,
            CommandLineApplication parentApplication = null)
        {
            bool isRootApp = parentApplication == null;

            CommandLineApplication app;

            ApplicationMetadataAttribute consoleApplicationAttribute = type.GetCustomAttribute<ApplicationMetadataAttribute>(false);
            
            if (isRootApp)
            {
                string rootName = $"dotnet {Assembly.GetEntryAssembly().GetName().Name}.dll";
                app = new CommandLineApplication(throwOnUnexpectedArg: _appSettings.ThrowOnUnexpectedArgument) { Name = rootName };
            }
            else
            {
                string subAppName = consoleApplicationAttribute?.Name ?? type.Name.ChangeCase(_appSettings.Case); 
                app = parentApplication.Command(subAppName, application => { });
            }

            app.HelpOption(Constants.HelpTemplate);

            app.Description = consoleApplicationAttribute?.Description;

            app.FullName = consoleApplicationAttribute?.Description;

            app.ExtendedHelpText = consoleApplicationAttribute?.ExtendedHelpText;

            app.Syntax = consoleApplicationAttribute?.Syntax;

            app.AllowArgumentSeparator = _appSettings.AllowArgumentSeparator;

            CommandCreator commandCreator = new CommandCreator(type, app, dependencyResolver, _appSettings);
            
            commandCreator.CreateDefaultCommand();

            commandCreator.CreateCommands();

            type.CreateSubApplications(_appSettings, app, dependencyResolver);

            return app;
        }
    }
}