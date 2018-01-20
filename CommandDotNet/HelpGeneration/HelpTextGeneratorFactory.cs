using CommandDotNet.Models;

namespace CommandDotNet.HelpGeneration
{
    public static class HelpTextGeneratorFactory
    {
        public static IHelpGenerator Create(HelpTextStyle helpTextStyle)
        {
            switch (helpTextStyle)
            {
                case HelpTextStyle.Standard:
                    return new StandardHelpTextGenerator();
                case HelpTextStyle.Tabular:
                    return new TabularHelpTextGenerator();
                default:
                    return new StandardHelpTextGenerator();
            }
        }
    }
}