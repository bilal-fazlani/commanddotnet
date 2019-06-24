// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandDotNet.MicrosoftCommandLineUtils
{
    public class CommandOperand : IOperand, ISettableArgument
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

        public void SetValues(List<string> values)
        {
            Values = values;
        }

        #region Obsolete members

        [Obsolete("do not use.  value is always true.")]
        public bool ShowInHelpText { get; set; } = true;

        [Obsolete("Use Arity.MaximumNumberOfValues > 1 instead")]
        public bool MultipleValues
        {
            get => Arity.AllowsZeroOrMore();
            set => Arity = value ? ArgumentArity.ZeroOrMore : ArgumentArity.ExactlyOne;
        }

        [Obsolete("Use Values.FirstOrDefault() instead.")]
        public string Value => Values.FirstOrDefault();

        #endregion

        public override string ToString()
        {
            return $"Operand: {new ArgumentTemplate(name:Name, typeDisplayName:TypeDisplayName)}";
        }
    }
}