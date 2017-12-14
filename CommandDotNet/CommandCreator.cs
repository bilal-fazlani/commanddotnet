using System;
using System.Collections.Generic;
using System.Reflection;
using CommandDotNet.Attributes;
using CommandDotNet.Models;
using Microsoft.Extensions.CommandLineUtils;

namespace CommandDotNet
{
    public class CommandCreator
    {
        private readonly Type _type;

        public CommandCreator(Type type)
        {
            _type = type;
        }

        public CommandLineApplication CreateCommand(AppSettings settings, string name)
        {
            CommandLineApplication command = new CommandLineApplication();
            
            List<ArgumentInfo> optionValues = _type.GetOptionValues(command, settings);
                            
            ApplicationMetadataAttribute consoleApplicationAttribute =
                _type.GetCustomAttribute<ApplicationMetadataAttribute>(false);

            command.Name = name ?? consoleApplicationAttribute?.Name;
            
            command.HelpOption(Constants.HelpTemplate);

            command.FullName = consoleApplicationAttribute?.Description;

            command.ExtendedHelpText = consoleApplicationAttribute?.ExtendedHelpText;
            
            _type.CreateDefaultSubCommand(command, settings, optionValues);
                
            _type.CreateSubCommands(command, settings, optionValues);

            return command;
        }
    }
}