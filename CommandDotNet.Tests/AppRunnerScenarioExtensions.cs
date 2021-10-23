using CommandDotNet.Execution;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;

namespace CommandDotNet.Tests
{
    public static class AppRunnerScenarioExtensions
    {
        public static AppRunner StopAfter(this AppRunner runner, MiddlewareStep step)
            => runner.Configure(cfg => cfg.UseMiddleware((c, n) => ExitCodes.Success, step+1));

        public static AppRunner StopAfter(this AppRunner runner, MiddlewareStages stage)
            => runner.StopAfter(new MiddlewareStep(stage));

        public static AppRunnerResult RunInMem(this AppRunner runner, string args)
        {
            return runner.RunInMem(args, Ambient.WriteLine);
        }

        public static AppRunnerResult RunInMem(this AppRunner runner, string[] args)
        {
            return runner.RunInMem(args, Ambient.WriteLine);
        }

        public static AppRunnerResult Verify(this AppRunner appRunner, IScenario scenario)
        {
            // use Test.Default to force testing of TestConfig.GetDefaultFromSubClass()
            return appRunner.Verify(Ambient.WriteLine, TestConfig.Default, scenario);
        }
    }
}