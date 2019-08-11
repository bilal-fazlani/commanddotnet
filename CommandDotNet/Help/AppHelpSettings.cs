namespace CommandDotNet.Help
{
    public class AppHelpSettings
    {
        /// <summary>When true, the help option will be included in the help for every command</summary>
        public bool PrintHelpOption { get; set; }

        /// <summary>Specify whether to use Basic or Detailed help mode. Default is Detailed.</summary>
        public HelpTextStyle TextStyle { get; set; } = HelpTextStyle.Detailed;

        /// <summary>Specify what AppName to use in the 'Usage:' example</summary>
        public UsageAppNameStyle UsageAppNameStyle { get; set; } = UsageAppNameStyle.Adaptive;

    }
}