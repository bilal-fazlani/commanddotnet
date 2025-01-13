using CommandDotNet.Extensions;
using JetBrains.Annotations;

namespace CommandDotNet;

/// <summary>
/// <see cref="Password"/> will capture input and only make it
/// available through a method to prevent displaying the data
/// via logging tools that use <see cref="ToString"/> or
/// reflect properties.<br/>
/// <see cref="Password"/> indicates intent to middleware
/// that may capture data, i.e. Prompting for missing arguments.
/// </summary>
[PublicAPI]
public class Password(string password)
{
    public static readonly string ValueReplacement = "*****";

    private readonly string _password = password.ThrowIfNull();

    public string GetPassword() => _password;

    public override string ToString() => _password.IsNullOrEmpty() ? "" : ValueReplacement;

    protected bool Equals(Password other) => _password == other._password;

    public override bool Equals(object? obj) =>
        obj is not null
        && (ReferenceEquals(this, obj)
            || obj.GetType() == GetType()
            && Equals((Password) obj));

    public override int GetHashCode() => _password.GetHashCode();
}