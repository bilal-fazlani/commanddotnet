// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandDotNet.MicrosoftCommandLineUtils
{
    public class CommandOption : IOption
    {
        public CommandOption(string template, IArgumentArity arity)
        {
            Template = template;
            Arity = arity;
            Values = new List<string>();

            var argumentTemplate = new ArgumentTemplate(template);
            Name = argumentTemplate.Name;
            ShortName = argumentTemplate.ShortName;
            SymbolName = argumentTemplate.SymbolName;
            TypeDisplayName = argumentTemplate.TypeDisplayName;
        }

        public string Name { get; set; }
        public string Description { get; set; }

        public string TypeDisplayName { get; set; }
        public IArgumentArity Arity { get; set; }
        public object DefaultValue { get; set; }
        public List<string> AllowedValues { get; set; }
        public List<string> Values { get; private set; }

        public string Template { get; set; }
        public string ShortName { get; set; }
        public string SymbolName { get; set; }

        public bool Inherited { get; set; }

        /// <summary>True when option is help or version</summary>
        public bool IsSystemOption { get; set; }

        public IEnumerable<string> Aliases
        {
            get
            {
                if (!Name.IsNullOrWhitespace()) yield return Name;
                if (!ShortName.IsNullOrWhitespace()) yield return ShortName;
                if (!SymbolName.IsNullOrWhitespace()) yield return SymbolName;
            }
        }

        public void SetValues(List<string> values)
        {
            Values = values;
        }

        public override string ToString()
        {
            return $"Option: {new ArgumentTemplate(Name, ShortName, SymbolName, TypeDisplayName)}";
        }

        #region Obsolete Members

        [Obsolete("Use Name instead")]
        public string LongName
        {
            get => Name;
            set => Name = value;
        }

        [Obsolete("Use Arity instead")]
        public CommandOptionType OptionType { get; }

        [Obsolete("Use TypeDisplayName instead")]
        public string ValueName
        {
            get => TypeDisplayName;
            set => TypeDisplayName = value;
        }

        [Obsolete("Use Arity.MaximumNumberOfValues > 1 instead")]
        public bool Multiple
        {
            get => Arity.AllowsZeroOrMore();
            set => Arity = value ? ArgumentArity.ZeroOrMore : ArgumentArity.ExactlyOne;
        }

        [Obsolete("do not use.  value is always true.")]
        public bool ShowInHelpText { get; set; } = true;

        [Obsolete("Use Values.Any() instead.")]
        public bool HasValue()
        {
            return Values.Any();
        }

        [Obsolete("Use Values.FirstOrDefault() instead.")]
        public string Value()
        {
            return Values.Any() ? Values[0] : null;
        }

        [Obsolete("Do not use. The method will be removed. Logic has been moved to CommandParser where similar logic for CommandOperand exists.")]
        public bool TryParse(string value)
        {
            if (Arity.AllowsZeroOrMore())
            {
                Values.Add(value);
            }
            else if (Arity.AllowsZeroOrOne())
            {
                if (Values.Any())
                {
                    return false;
                }
                Values.Add(value);
            }
            else if (Arity.AllowsNone())
            {
                if (value != null)
                {
                    return false;
                }
                // Add a value to indicate that this option was specified
                Values.Add("true");
            }
            return true;
        }

        #endregion
    }
}