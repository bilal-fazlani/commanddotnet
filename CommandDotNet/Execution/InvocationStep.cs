﻿using System;
using CommandDotNet.Extensions;
using static System.Environment;

namespace CommandDotNet.Execution;

public class InvocationStep(Command command, IInvocation invocation) : IIndentableToString
{
    private IInvocation _invocation = invocation ?? throw new ArgumentNullException(nameof(invocation));

    // begin-snippet: InvocationStep-properties

    /// <summary>
    /// The instance of the class containing the invocation method.<br/>
    /// Populated during <see cref="MiddlewareStages.BindValues"/>
    /// </summary>
    public object? Instance { get; set; }

    /// <summary>
    /// The command the invocation will fulfill.<br/>
    /// Populated during <see cref="MiddlewareStages.ParseInput"/>
    /// </summary>
    public Command Command { get; } = command ?? throw new ArgumentNullException(nameof(command));

    /// <summary>
    /// The <see cref="IInvocation"/> to be invoked for the command<br/>
    /// Populated during <see cref="MiddlewareStages.ParseInput"/>
    /// </summary>
    public IInvocation Invocation
    {
        get => _invocation;
        set => _invocation = value ?? throw new ArgumentNullException(nameof(value));
    }
        
    // end-snippet

    public override string ToString() => ToString(new Indent());

    public string ToString(Indent indent)
    {
        return $"{nameof(InvocationStep)}:{NewLine}" +
               $"{indent}{nameof(Command)}:{Command.Name}{NewLine}" +
               $"{indent}{nameof(Invocation)}:{Invocation.ToIndentedString(indent.Increment())}{NewLine}" +
               $"{indent}{nameof(Instance)}:{Instance}";
    }
}