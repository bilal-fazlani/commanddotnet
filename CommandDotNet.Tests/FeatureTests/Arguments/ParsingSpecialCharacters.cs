using CommandDotNet.Attributes;
using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.Tests.Utils;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class ParsingSpecialCharacters : ScenarioTestBase<ParsingSpecialCharacters>
    {
        public ParsingSpecialCharacters(ITestOutputHelper output) : base(output)
        {
        }

        public static Scenarios Scenarios =>
            new Scenarios
            {
                new Given<App>("exec - trims double quotes from beginning and end of string")
                {
                    WhenArgsArray = new []{ "Do", "\"some \" text\"" },
                    Then =
                    {
                        Outputs = { "some \" text" }
                    }
                },
                new Given<App>("exec - trims single quotes from beginning and end of string")
                {
                    WhenArgsArray = new []{ "Do", "\'some ' text\'" },
                    Then =
                    {
                        Outputs = { "some ' text" }
                    }
                },
                new Given<App>("exec - brackets should be retained in the test")
                {
                    WhenArgsArray = new []{ "Do", "[some (parenthesis) {curly} and [bracketed] text]" },
                    Then =
                    {
                        Outputs = { "[some (parenthesis) {curly} and [bracketed] text]" }
                    }
                },
                new Given<App>("exec - special characters should be retained")
                {
                    WhenArgsArray = new []{ "Do", "~!@#$%^&*()_=+[]\\{} |;':\",./<>?" },
                    Then =
                    {
                        Outputs = { "~!@#$%^&*()_= +[]\\{} |;':\",./<>?" }
                    }
                },
            };

        public class App
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            public void Do([Argument] string arg)
            {
                TestOutputs.Capture(arg);
            }
        }
    }
}