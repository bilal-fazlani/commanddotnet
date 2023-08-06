// copied from: https://raw.githubusercontent.com/dotnet/command-line-api/main/src/System.CommandLine.Suggest/RegistrationPair.cs
// via: DotNetSuggestSync test class

#pragma warning disable CS8600
#pragma warning disable CS8603
#pragma warning disable CS8625
// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
// ReSharper disable CheckNamespace

namespace System.CommandLine.Suggest
{
    public class Registration
    {
        public Registration(string executablePath)
        {
            ExecutablePath = executablePath ?? throw new ArgumentNullException(nameof(executablePath));
        }

        public string ExecutablePath { get; }
    }
}
