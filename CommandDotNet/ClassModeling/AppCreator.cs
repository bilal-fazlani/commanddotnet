using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;

namespace CommandDotNet.ClassModeling
{
    internal class AppCreator
    {
        private readonly AppSettings _appSettings;

        public AppCreator(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public ICommand CreateRootCommand(Type type, CommandContext commandContext)
        { 
            return CreateCommand(type, commandContext, null);
            // return new ClassCommandDef(type, executionConfig).ToCommand(null, executionConfig);
        }

        private Command CreateCommand(
            Type type,
            CommandContext commandContext,
            Command parentCommand)
        {
            bool isRootCommand = parentCommand == null;

            var applicationAttribute = type.GetCustomAttribute<ApplicationMetadataAttribute>(false);

            string appName = applicationAttribute?.Name ?? type.Name.ChangeCase(_appSettings.Case);

            var command = isRootCommand 
                ? new Command(appName, type) 
                : parentCommand.AddCommand(appName, type);

            CommandCreator commandCreator = new CommandCreator(type, command, commandContext);
            
            commandCreator.CreateDefaultCommand(applicationAttribute);

            commandCreator.CreateCommands();

            CreateSubApplications(type, command, commandContext);

            return command;
        }
        
        private void CreateSubApplications(Type type,
            Command parentApplication,
            CommandContext commandContext)
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
                appCreator.CreateCommand(submoduleType, commandContext, parentApplication);
            }
        }
    }
}