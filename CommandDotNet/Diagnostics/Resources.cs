// ReSharper disable CheckNamespace

using CommandDotNet.Diagnostics;

namespace CommandDotNet
{
    public partial class Resources
    {
        public virtual string Command_version => "version";
        public virtual string Command_version_description => "Show version information";

        public virtual string CommandLogger_Original_input => "Original input";
        public virtual string CommandLogger_Tool_version => "Tool version";
        public virtual string CommandLogger_DotNet_version => ".Net version";
        public virtual string CommandLogger_OS_version => "OS version";
        public virtual string CommandLogger_Machine => "Machine";
        public virtual string CommandLogger_Username => "Username";

        public string Error_CommandLogger_has_not_been_registered() =>
            $"{nameof(CommandLoggerMiddleware)} has not been registered. " +
            $"Try `appRunner.{nameof(AppRunnerConfigExtensions.UseCommandLogger)}()`";
    }
}