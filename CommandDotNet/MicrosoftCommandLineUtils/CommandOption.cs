// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandDotNet.MicrosoftCommandLineUtils
{
    public class CommandOption : IArgument
    {
        public CommandOption(string template, CommandOptionType optionType)
        {
            Template = template;
            OptionType = optionType;
            Values = new List<string>();

            var argumentTemplate = new ArgumentTemplate(template);
            Name = argumentTemplate.Name;
            ShortName = argumentTemplate.ShortName;
            SymbolName = argumentTemplate.SymbolName;
            TypeDisplayName = argumentTemplate.TypeDisplayName;
        }

        /// <summary>True when option is help or version</summary>
        public bool IsSystemOption { get; set; }

        public string Template { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string SymbolName { get; set; }
        public string Description { get; set; }
        public List<string> Values { get; internal set; }
        public CommandOptionType OptionType { get; private set; }
        public bool ShowInHelpText { get; set; } = true;
        public bool Inherited { get; set; }
        
        public string TypeDisplayName { get; set; }
        public object DefaultValue { get; set; }
        public bool Multiple { get; set; }
        public List<string> AllowedValues { get; set; }

        [Obsolete("Use Name instead")]
        public string LongName
        {
            get => Name;
            set => Name = value;
        }

        [Obsolete("Use TypeDisplayName instead")]
        public string ValueName
        {
            get => TypeDisplayName;
            set => TypeDisplayName = value;
        }

        public bool TryParse(string value)
        {
            switch (OptionType)
            {
                case CommandOptionType.MultipleValue:
                    Values.Add(value);
                    break;
                case CommandOptionType.SingleValue:
                    if (Values.Any())
                    {
                        return false;
                    }
                    Values.Add(value);
                    break;
                case CommandOptionType.NoValue:
                    if (value != null)
                    {
                        return false;
                    }
                    // Add a value to indicate that this option was specified
                    Values.Add("on");
                    break;
            }
            return true;
        }

        public bool HasValue()
        {
            return Values.Any();
        }

        public string Value()
        {
            return HasValue() ? Values[0] : null;
        }

        public override bool Equals(object obj)
        {
            if (obj is CommandOption commandOption)
            {
                return commandOption.Template == Template;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Template?.GetHashCode() ?? 0;
        }
    }
}