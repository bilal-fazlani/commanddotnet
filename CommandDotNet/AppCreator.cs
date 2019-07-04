using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CommandDotNet.Attributes;
using CommandDotNet.Extensions;
using CommandDotNet.MicrosoftCommandLineUtils;
using CommandDotNet.Models;

namespace CommandDotNet
{
    internal class AppCreator
    {
        private readonly AppSettings _appSettings;
        private readonly IOptionSource[] _optionSources;

        public AppCreator(AppSettings appSettings)
        {
            _appSettings = appSettings;

            _optionSources = new IOptionSource[]
            {
                new HelpOptionSource(_appSettings),
                new VersionOptionSource(_appSettings)
            };
        }

        public CommandLineApplication CreateApplication(
            Type type,
            IDependencyResolver dependencyResolver,
            CommandLineApplication parentApplication = null)
        {
            bool isRootApp = parentApplication == null;

            var applicationAttribute = type.GetCustomAttribute<ApplicationMetadataAttribute>(false);

            string appName = applicationAttribute?.Name ?? type.Name.ChangeCase(_appSettings.Case);

            var app = isRootApp 
                ? new CommandLineApplication(_appSettings, appName, type) 
                : parentApplication.Command(appName, type);

            //_optionSources.ForEach(s => s.AddOption(app));

            CommandCreator commandCreator = new CommandCreator(type, app, dependencyResolver, _appSettings, _optionSources);
            
            commandCreator.CreateDefaultCommand(applicationAttribute);

            commandCreator.CreateCommands();

            CreateSubApplications(type, app, dependencyResolver);

            return app;
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
                // TODO: optimization - return a LazyAppCreator, parsing sub applications only as needed
                //       the current code creates all sub applications
                //       for an app like git with a lot of commands this can have a noticeable impact
                //       this accounts for ~10% of the run time across all feature tests

                AppCreator appCreator = new AppCreator(_appSettings);
                appCreator.CreateApplication(submoduleType, dependencyResolver, parentApplication);
            }
        }
    }
}