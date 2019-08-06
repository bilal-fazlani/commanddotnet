using Xunit.Abstractions;

namespace CommandDotNet.Tests.ScenarioFramework
{
    public class TestBase
    {
        protected readonly ITestOutputHelper TestOutputHelper;

        protected TestBase(ITestOutputHelper testOutputHelper)
        {
            TestOutputHelper = testOutputHelper;
        }

        protected void Verify(IScenario scenario)
        {
            if (scenario.And.AppSettings == null)
            {
                scenario.And.AppSettings = TestAppSettings.TestDefault;
            }

            var appRunner = new AppRunner(
                scenario.AppType,
                scenario.And.AppSettings ?? TestAppSettings.TestDefault);

            appRunner.VerifyScenario(TestOutputHelper, scenario);
        }
    }
}