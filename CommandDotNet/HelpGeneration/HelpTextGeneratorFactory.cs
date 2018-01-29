using CommandDotNet.Models;

namespace CommandDotNet.HelpGeneration
{
    public static class HelpTextGeneratorFactory
    {
        public static IHelpGenerator Create(AppSettings appSettings)
        {
            switch (appSettings.HelpTextStyle)
            {
                case HelpTextStyle.Basic:
                    return new BasicHelpTextGenerator(appSettings);
                case HelpTextStyle.Detailed:
                    return new DetailedHelpTextGenerator(appSettings);
                default:
                    return new BasicHelpTextGenerator(appSettings);
            }
        }
    }
}