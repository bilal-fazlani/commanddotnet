using System.Collections.Generic;
using System.Collections.Specialized;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ArgumentDefaults
{
    public class DefaultFromMultipleSourcesTests
    {
        public DefaultFromMultipleSourcesTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void UsesDefaultFromTheFirstSourceAvailable()
        {
            new AppRunner<App>()
                .UseDefaultsFromAppSetting(new NameValueCollection
                {
                    {"opt1", "from AppSetting"}
                })
                .UseDefaultsFromEnvVar(new Dictionary<string,string>
                {
                    { "opt1", "from EnvVar" },
                    { "opt2", "from EnvVar" }
                })
                .UseDefaultsFromConfig(arg => Config(arg, "from Config"))
                .Verify(new Scenario
                {
                    When = { Args = "Do" },
                    Then = { AssertContext = ctx => ctx.ParamValuesShouldBe("from AppSetting", "from EnvVar", "from Config") }
                });
        }


        private static ArgumentDefault? Config(IArgument argument, string value)
        {
            return argument.TypeInfo == TypeInfo.Flag
                ? null
                : new ArgumentDefault("test", "key", value);
        }

        private class App
        {
            public void Do(
                [AppSetting("opt1")]
                [EnvVar("opt1")]
                [Option] string option1 = "from param",

                [AppSetting("opt2")]
                [EnvVar("opt2")]
                [Option] string option2 = "from param",

                [AppSetting("opt3")]
                [EnvVar("opt3")]
                [Option] string option3 = "from param"
            )
            {

            }
        }
    }
}