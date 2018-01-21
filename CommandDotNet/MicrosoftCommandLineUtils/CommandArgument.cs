// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;

namespace CommandDotNet.MicrosoftCommandLineUtils
{
    public class CommandArgument : IParameter
    {
        public CommandArgument()
        {
            Values = new List<string>();
        }

        public string Name { get; set; }
        public bool ShowInHelpText { get; set; } = true;
        public string Description { get; set; }
        public List<string> Values { get; internal set; }
        public bool MultipleValues { get; set; }
        public string TypeDisplayName { get; set; }
        public string DefaultValue { get; set; }
        
        public string Value
        {
            get
            {
                return Values.FirstOrDefault();
            }
        }
    }
}