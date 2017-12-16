using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using CommandDotNet.Attributes;
using CommandDotNet.Models;
using Microsoft.Extensions.CommandLineUtils;

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
            CommandLineApplication parentApplication = null)
        {
            bool isRootApp = parentApplication == null;

            CommandLineApplication command;

            ApplicationMetadataAttribute consoleApplicationAttribute = type.GetCustomAttribute<ApplicationMetadataAttribute>(false);
            
            if (isRootApp)
            {
                string rootName = $"dotnet {Assembly.GetCallingAssembly().GetName().Name}.dll";
                command = new CommandLineApplication { Name = rootName };
            }
            else
            {
                string subAppName = consoleApplicationAttribute?.Name ?? type.Name; 
                command = parentApplication.Command(subAppName, application => { });
            }

            List<ArgumentInfo> optionValues = type.GetOptionValues(command, _appSettings);

            command.HelpOption(Constants.HelpTemplate);

            command.Description = consoleApplicationAttribute?.Description;
            
            command.FullName = consoleApplicationAttribute?.Description;

            command.ExtendedHelpText = consoleApplicationAttribute?.ExtendedHelpText;

            command.AllowArgumentSeparator = true;

            CommandCreator commandCreator = new CommandCreator(type, _appSettings);

            commandCreator.CreateDefaultSubCommand(command, optionValues);

            commandCreator.CreateSubCommands(command, optionValues);

            type.CreateSubApplications(_appSettings, command);

            return command;
        }
    }
}