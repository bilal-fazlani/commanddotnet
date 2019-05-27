namespace CommandDotNet.Models
{
    public class AppHelpSettings
    {
        public HelpTextStyle TextStyle { get; set; } = HelpTextStyle.Detailed;
        public UsageAppNameStyle UsageAppNameStyle { get; set; } = UsageAppNameStyle.Adaptive;

    }
}