namespace CommandDotNet.Help
{
    public class AppHelpSettings
    {
        public bool PrintHelpOption { get; set; } = false;
        public HelpTextStyle TextStyle { get; set; } = HelpTextStyle.Detailed;
        public UsageAppNameStyle UsageAppNameStyle { get; set; } = UsageAppNameStyle.Adaptive;

    }
}