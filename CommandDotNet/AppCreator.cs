using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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
            
            string assemblyName = $"dotnet {Assembly.GetEntryAssembly().GetName().Name}.dll";
            
            string appName = consoleApplicationAttribute?.Name ?? type.Name.ChangeCase(_appSettings.Case); 
            
            
            if (isRootApp)
            {
                app = new CommandLineApplication(throwOnUnexpectedArg: _appSettings.ThrowOnUnexpectedArgument) { Name = assemblyName };
                AddVersion(app);
            }
            else
            {
                app = parentApplication.Command(appName, application => { });
            }

            app.HelpOption(Constants.HelpTemplate);

            app.Description = consoleApplicationAttribute?.Description;

            app.FullName = appName;

            app.ExtendedHelpText = consoleApplicationAttribute?.ExtendedHelpText;

            app.Syntax = consoleApplicationAttribute?.Syntax;

            app.AllowArgumentSeparator = _appSettings.AllowArgumentSeparator;

            CommandCreator commandCreator = new CommandCreator(type, app, dependencyResolver, _appSettings);
            
            commandCreator.CreateDefaultCommand();

            commandCreator.CreateCommands();

            CreateSubApplications(type, app, dependencyResolver);

            return app;
        }

        private void AddVersion(CommandLineApplication app)
        {
            if (_appSettings.EnableVersionOption)
            {
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);
                app.VersionOption("-v | --version", shortFormVersion: fvi.ProductVersion);
            }
        }
        
        private void CreateSubApplications(Type type,
            CommandLineApplication parentApplication,
            IDependencyResolver dependencyResolver)
        {
            IEnumerable<Type> propertySubmodules = 
                type.GetDeclaredProperties<SubCommandAttribute>()
                .Select(p => p.PropertyType);
            
            IEnumerable<Type> inlineClassSubmodules = type
                .GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                .Where(x=> x.HasAttribute<SubCommandAttribute>())
                .Where(x=> !x.IsCompilerGenerated())
                .Where(x=> !typeof(IAsyncStateMachine).IsAssignableFrom(x));
            
            foreach (Type submoduleType in propertySubmodules.Union(inlineClassSubmodules))
            {
                AppCreator appCreator = new AppCreator(_appSettings);
                appCreator.CreateApplication(submoduleType, dependencyResolver, parentApplication);
            }
        }
    }
}