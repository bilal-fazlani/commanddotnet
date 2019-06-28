// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace CommandDotNet.MicrosoftCommandLineUtils
{
    public class CommandOperand : IOperand
    {
        public CommandOperand()
        {
            Values = new List<string>();
        }

        public string Name { get; set; }
        public string Description { get; set; }

        public string TypeDisplayName { get; set; }
        public IArgumentArity Arity { get; set; }
        public object DefaultValue { get; set; }
        public List<string> AllowedValues { get; set; }
        public List<string> Values { get; private set; }

        public IEnumerable<string> Aliases
        {
            get { yield return Name; }
        }

        public IServices Services { get; } = new Services();

        public void SetValues(List<string> values)
        {
            Values = values;
        }

        public override string ToString()
        {
            return $"Operand: {new ArgumentTemplate(name:Name, typeDisplayName:TypeDisplayName)}";
        }
    }
}