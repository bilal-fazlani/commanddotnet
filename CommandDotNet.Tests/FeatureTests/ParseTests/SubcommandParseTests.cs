using System.Threading.Tasks;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ParseTests
{
    public class SubcommandParseTests
    {
        public SubcommandParseTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void SubcommandsCannotFollowOperandsOfDefaultCommand()
        {
            var results = new AppRunner<DefaultApp>()
                .Verify(new Scenario
                {
                    When = { Args = "1 Do" },
                    Then =
                    {
                        AssertContext = ctx =>
                        {
                            var invocation = ctx.GetCommandInvocationInfo();
                            invocation.MethodInfo!.Name.Should().Be("Default");
                            ctx.ParamValuesShouldBe("1", "Do", null, null);
                        }
                    }
                });
        }

        [Fact]
        public void SubcommandsCannotFollowOptionsOfDefaultCommand()
        {
            var results = new AppRunner<DefaultApp>()
                .Verify(new Scenario
                {
                    When = { Args = "--opt1 lala Do" },
                    Then =
                    {
                        AssertContext = ctx =>
                        {
                            var invocation = ctx.GetCommandInvocationInfo();
                            invocation.MethodInfo!.Name.Should().Be("Default");
                            ctx.ParamValuesShouldBe("Do", null, "lala", null);
                        }
                    }
                });
        }

        [Fact]
        public void SubcommandsCanFollowInterceptorOptions()
        {
            var results = new AppRunner<DefaultApp>()
                .Verify(new Scenario
                {
                    When = { Args = "--localIOption lala Do" },
                    Then =
                    {
                        AssertContext = ctx =>
                        {
                            var invocation = ctx.GetCommandInvocationInfo();
                            invocation.MethodInfo!.Name.Should().Be("Do");
                            ctx.ParamValuesShouldBe(null, null, null, null);
                            ctx.ParamValuesShouldBe<DefaultApp>("lala", null);
                        }
                    }
                });
        }

        [Fact]
        public void SubcommandsCannotFollowInheritedInterceptorOptions()
        {
            var results = new AppRunner<DefaultApp>()
                .Verify(new Scenario
                {
                    When = { Args = "--inheritedIOption lala Do" },
                    Then =
                    {
                        AssertContext = ctx =>
                        {
                            var invocation = ctx.GetCommandInvocationInfo();
                            invocation.MethodInfo!.Name.Should().Be("Default");
                            ctx.ParamValuesShouldBe("Do", null, null, null);
                            ctx.ParamValuesShouldBe<DefaultApp>(null, "lala");
                        }
                    }
                });
        }

        [Fact]
        public void SubcommandsWithOptionPrefixGeneratesSuggestion()
        {
            new AppRunner<DefaultApp>().Verify(new Scenario
            {
                When = { Args = "--Do" },
                Then =
                {
                    ExitCode = 1,
                    OutputContainsTexts = { @"Unrecognized option '--Do'
If you intended to use the 'Do' command, try again with the following

Do" }
                }
            });
        }

        private class DefaultApp
        {
            public Task<int> Interceptor(InterceptorExecutionDelegate next, 
                [Option] string? localIOption,
                [Option(AssignToExecutableSubcommands = true)] string? inheritedIOption)
            {
                return next();
            }

            [DefaultCommand]
            public void Default(string? opd1, string? opd2, [Option] string? opt1, [Option] string? opt2)
            {
            }

            public void Do(string? opd1, string? opd2, [Option] string? opt1, [Option] string? opt2)
            {
            }
        }
    }
}