using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CommandDotNet.Attributes;
using CommandDotNet.Exceptions;
using CommandDotNet.Extensions;
using CommandDotNet.MicrosoftCommandLineUtils;
using CommandDotNet.Models;

namespace CommandDotNet
{
    internal class AppCreator
    {
        private readonly AppSettings _appSettings;
        private readonly Assembly _hostAssembly;

        public AppCreator(AppSettings appSettings)
        {
            _appSettings = appSettings;
            _hostAssembly = Assembly.GetEntryAssembly();
        }

        public CommandLineApplication CreateApplication(
            Type type,
            IDependencyResolver dependencyResolver,
            CommandLineApplication parentApplication = null)
        {
            bool isRootApp = parentApplication == null;

            CommandLineApplication app;

            var applicationAttribute = type.GetCustomAttribute<ApplicationMetadataAttribute>(false);

            string appName = applicationAttribute?.Name ?? type.Name.ChangeCase(_appSettings.Case);

            if (isRootApp)
            {
                app = new CommandLineApplication(_appSettings)
                {
                    Name = appName, 
                    CustomAttributeProvider = type
                };
                AddVersion(app);
            }
            else
            {
                app = parentApplication.Command(appName);
            }

            CommandCreator commandCreator = new CommandCreator(type, app, dependencyResolver, _appSettings);
            
            commandCreator.CreateDefaultCommand(applicationAttribute);

            commandCreator.CreateCommands();

            CreateSubApplications(type, app, dependencyResolver);

            return app;
        }

        private void AddVersion(CommandLineApplication app)
        {
            if (_appSettings.EnableVersionOption)
            {
                if (_hostAssembly == null)
                {
                    throw new AppRunnerException(
                        "Unable to determine version because Assembly.GetEntryAssembly() is null. " +
                        "This is a known issue when running unit tests in .net framework. " +
                        "Consider disabling for test runs. " +
                        "https://tinyurl.com/y6rnjqsg");
                }
                
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(_hostAssembly.Location);
                var filename = Path.GetFileName(_hostAssembly.Location);
                string version = fvi.ProductVersion;
                app.VersionOption(
                    "-v | --version", 
                    () =>
                    {
                        _appSettings.Out.WriteLine(filename);
                        _appSettings.Out.WriteLine(version);
                    });
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