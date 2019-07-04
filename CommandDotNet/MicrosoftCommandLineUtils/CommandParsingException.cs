// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace CommandDotNet.MicrosoftCommandLineUtils
{
    internal class CommandParsingException : Exception
    {
        public ICommand Command { get; }

        public CommandParsingException(ICommand command, string message)
            : base(message)
        {
            Command = command;
        }
    }
}