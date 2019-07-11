// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using CommandDotNet.Execution;

namespace CommandDotNet
{
    public class Operand : IOperand
    {
        public Operand()
        {
            Values = new List<string>();
        }

        public string Name { get; set; }
        public string Description { get; set; }

        public ITypeInfo TypeInfo { get; set; }
        public IArgumentArity Arity { get; set; }
        public object DefaultValue { get; set; }
        public List<string> AllowedValues { get; set; }
        public List<string> Values { get; private set; }

        public IEnumerable<string> Aliases
        {
            get { yield return Name; }
        }

        public IContextData ContextData { get; } = new ContextData();

        public void SetValues(List<string> values)
        {
            Values = values;
        }

        public override string ToString()
        {
            return $"Operand: {new ArgumentTemplate(name:Name, typeDisplayName:TypeInfo.DisplayName)}";
        }
    }
}