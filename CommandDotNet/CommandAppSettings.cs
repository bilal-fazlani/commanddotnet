﻿using CommandDotNet.Extensions;
using JetBrains.Annotations;

namespace CommandDotNet;

[PublicAPI]
public class CommandAppSettings : IIndentableToString
{
    /// <summary>When true, methods on base classes will be included as commands.</summary>
    public bool InheritCommandsFromBaseClasses { get; set; }

    public override string ToString() => ToString(new Indent());

    public string ToString(Indent indent) => this.ToStringFromPublicProperties(indent);
}