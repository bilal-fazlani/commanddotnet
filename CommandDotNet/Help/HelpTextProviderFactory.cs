namespace CommandDotNet.Help
{
    internal static class HelpTextProviderFactory
    {
        internal static IHelpProvider Create(AppSettings appSettings)
        {
            switch (appSettings.Help.TextStyle)
            {
                case HelpTextStyle.Basic:
                    return new BasicHelpTextProvider(appSettings);
                case HelpTextStyle.Detailed:
                    return new HelpTextProvider(appSettings);
                default:
                    return new HelpTextProvider(appSettings);
            }
        }
    }
}