using System;
using System.Collections.Generic;
using CommandDotNet.Execution;

namespace CommandDotNet.ClassModeling.Definitions;

/// <summary>
/// Registry for source-generated command definition builders.
/// Generated code registers itself here via module initializers, eliminating runtime reflection.
/// </summary>
internal static class CommandDefRegistry
{
    private static readonly Dictionary<Type, Func<CommandContext, ICommandDef>> Builders = new();

    /// <summary>
    /// Registers a generated builder for a command type.
    /// Called by source-generated code via module initializer.
    /// </summary>
    internal static void Register<TCommand>(Func<CommandContext, ICommandDef> builder)
    {
        Builders[typeof(TCommand)] = builder ?? throw new ArgumentNullException(nameof(builder));
    }

    /// <summary>
    /// Attempts to get a registered builder for the given command type.
    /// Returns null if no generated builder exists (will fall back to reflection).
    /// </summary>
    internal static Func<CommandContext, ICommandDef>? TryGetBuilder(Type commandType)
    {
        return Builders.TryGetValue(commandType, out var builder) ? builder : null;
    }

    /// <summary>
    /// Gets count of registered builders (for diagnostics/testing)
    /// </summary>
    internal static int RegisteredCount => Builders.Count;

    /// <summary>
    /// Clears all registrations (for testing only)
    /// </summary>
    internal static void Clear() => Builders.Clear();
}
