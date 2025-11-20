using System;
using JetBrains.Annotations;

namespace CommandDotNet;

/// <summary>
/// Specifies a default group name for all options defined in an <see cref="IArgumentModel"/>.
/// This group will be inherited by all option properties in the model and any nested models,
/// unless overridden by a property-level <see cref="OptionAttribute.Group"/> or a nested model's
/// <see cref="ArgumentGroupAttribute"/>.
/// </summary>
[PublicAPI]
[AttributeUsage(AttributeTargets.Class)]
public class ArgumentGroupAttribute : Attribute
{
    /// <summary>
    /// The default group name for all options in this argument model.
    /// </summary>
    public string GroupName { get; }

    /// <summary>
    /// Specifies a default group name for all options defined in this <see cref="IArgumentModel"/>.
    /// </summary>
    /// <param name="groupName">The group name to apply to all options in this model</param>
    public ArgumentGroupAttribute(string groupName)
    {
        GroupName = groupName ?? throw new ArgumentNullException(nameof(groupName));
    }
}

