﻿using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace CommandDotNet;

/// <summary>
/// Used to determine the position of <see cref="Operand"/>s, <see cref="Option"/>s and nested <see cref="IArgumentModel"/>s within the class.
/// </summary>
/// <param name="__callerLineNumber">
/// The value is defaulted by <see cref="CallerLineNumberAttribute"/>.  Leave blank to let the position of the property determine the order.
/// </param>
[PublicAPI]
[AttributeUsage(AttributeTargets.Property)]
// ReSharper disable once InconsistentNaming
public class OrderByPositionInClassAttribute([CallerLineNumber] int __callerLineNumber = 0) : Attribute
{
    public int CallerLineNumber { get; } = __callerLineNumber;
}