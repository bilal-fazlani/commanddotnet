using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Builders;
using JetBrains.Annotations;

namespace CommandDotNet.Extensions;

[PublicAPI]
public static class CustomAttributesContainerExtensions
{
    public static bool HasAttribute<T>(this ICustomAttributesContainer container) where T : Attribute => 
        container.ThrowIfNull().CustomAttributes.HasAttribute<T>();

    public static T? GetCustomAttribute<T>(this ICustomAttributesContainer container) where T : Attribute =>
        container
            .GetCustomAttributes<T>()
            .SingleOrDefaultOrThrow(
                () => throw new InvalidConfigurationException($"attempted to get a single {typeof(T).Name} from {container} but multiple exist"));

    public static IEnumerable<T> GetCustomAttributes<T>(this ICustomAttributesContainer container) where T : Attribute => 
        container.ThrowIfNull().CustomAttributes.GetCustomAttributes(typeof(T), false).Cast<T>();
}