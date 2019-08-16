using CommandDotNet.Tests.ScenarioFramework;
using Xunit.Abstractions;

namespace CommandDotNet.Tests
{
    public class TestBase
    {
        protected readonly ITestOutputHelper TestOutputHelper;

        protected TestBase(ITestOutputHelper testOutputHelper)
        {
            TestOutputHelper = testOutputHelper;
        }

        protected void Verify(IScenarioForApp scenario)
        {
            if (scenario.Given.AppSettings == null)
            {
                scenario.Given.AppSettings = TestAppSettings.TestDefault;
            }

            var appRunner = new AppRunner(
                scenario.AppType,
                scenario.Given.AppSettings ?? TestAppSettings.TestDefault);

            appRunner.VerifyScenario(TestOutputHelper, scenario);
        }
    }
}