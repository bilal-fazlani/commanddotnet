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
                case HelpTextStyle.Tabular:
                    return new TabularHelpTextGenerator();
                case HelpTextStyle.Standard:
                    return new StandardHelpTextGenerator();
                default:
                    return new BasicHelpTextGenerator();
            }
        }
    }
}