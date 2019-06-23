// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandDotNet.MicrosoftCommandLineUtils
{
    public class CommandOperand : IArgument
    {
        public CommandOperand()
        {
            Values = new List<string>();
        }

        public string Name { get; set; }
        public bool ShowInHelpText { get; set; } = true;
        public string Description { get; set; }
        [Obsolete("Use Arity.MaximumNumberOfValues > 1 instead")]
        public bool MultipleValues 
        {
            get => Arity.AllowsZeroOrMore();
            set => Arity = value ? ArgumentArity.ZeroOrMore : ArgumentArity.ExactlyOne;
        }
        public IArgumentArity Arity { get; set; }
        public string TypeDisplayName { get; set; }
        public object DefaultValue { get; set; }
        public List<string> AllowedValues { get; set; }
        
        public string Value => Values.FirstOrDefault();
        public List<string> Values { get; internal set; }
    }
}