// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using CommandDotNet.Tokens;

namespace CommandDotNet.Parsing
{
    internal class CommandParsingException : Exception
    {
        public Command Command { get; }

        public Token UnrecognizedArgument { get; }

        public CommandParsingException(Command command, string message, Token unrecognizedArgument = null)
            : base(message)
        {
            Command = command;
            UnrecognizedArgument = unrecognizedArgument;
        }
    }
}