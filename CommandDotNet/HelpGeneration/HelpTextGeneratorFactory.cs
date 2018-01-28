using CommandDotNet.Models;

namespace CommandDotNet.HelpGeneration
{
    public static class HelpTextGeneratorFactory
    {
        public static IHelpGenerator Create(HelpTextStyle helpTextStyle)
        {
            switch (helpTextStyle)
            {
                case HelpTextStyle.Basic:
                    return new BasicHelpTextGenerator();
                case HelpTextStyle.Detailed:
                    return new DetailedHelpTextGenerator();
                default:
                    return new BasicHelpTextGenerator();
            }
        }
    }
}