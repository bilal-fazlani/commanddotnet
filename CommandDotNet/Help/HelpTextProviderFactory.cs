namespace CommandDotNet.Help
{
    internal static class HelpTextProviderFactory
    {
        internal static IHelpProvider Create(AppSettings appSettings)
        {
            if (appSettings.CustomHelpProvider != null)
            {
                return appSettings.CustomHelpProvider;
            }
            
            switch (appSettings.Help.TextStyle)
            {
                case HelpTextStyle.Basic:
                    return new BasicHelpTextProvider(appSettings);
                case HelpTextStyle.Detailed:
                    return new DetailedHelpTextProvider(appSettings);
                default:
                    return new BasicHelpTextProvider(appSettings);
            }
        }
    }
}