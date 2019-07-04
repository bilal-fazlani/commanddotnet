// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

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
        public object DefaultValue { get; set; } = DBNull.Value;
        public List<string> AllowedValues { get; set; }
        public List<string> Values { get; private set; }

        public string Template { get; set; }
        public string ShortName { get; set; }
        public string SymbolName { get; set; }

        public bool Inherited { get; set; }

        /// <summary>True when option is help or version</summary>
        public bool IsSystemOption { get; set; }
        public Action InvokeAsCommand { get; set; }

        public IEnumerable<string> Aliases
        {
            get
            {
                if (!Name.IsNullOrWhitespace()) yield return Name;
                if (!ShortName.IsNullOrWhitespace()) yield return ShortName;
                if (!SymbolName.IsNullOrWhitespace()) yield return SymbolName;
            }
        }

        public IContextData ContextData { get; } = new ContextData();

        public void SetValues(List<string> values)
        {
            Values = values;
        }

        public override string ToString()
        {
            return $"Option: {new ArgumentTemplate(Name, ShortName, SymbolName, TypeDisplayName)}";
        }
    }
}