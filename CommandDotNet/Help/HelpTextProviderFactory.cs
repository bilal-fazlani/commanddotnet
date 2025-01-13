namespace CommandDotNet.Help;

internal static class HelpTextProviderFactory
{
    internal static IHelpProvider Create(AppSettings appSettings) =>
        appSettings.Help.TextStyle switch
        {
            HelpTextStyle.Basic => new BasicHelpTextProvider(appSettings),
            HelpTextStyle.Detailed => new HelpTextProvider(appSettings),
            _ => new HelpTextProvider(appSettings)
        };
}