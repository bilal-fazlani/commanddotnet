using System.Threading.Tasks;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ClassCommands
{
    public class InterceptorExecutionMultilevelNestingTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public InterceptorExecutionMultilevelNestingTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void WhenChildCommandsAreNotRequested_TheirInterceptorsAreNotExecuted()
        {
            new AppRunner<App>()
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    WhenArgs = "--name gramps Greet",
                    Then =
                    {
                        Outputs =
                        {
                            "Hello, my name is gramps. My child is null. My grandchild is null."
                        }
                    }
                });
        }

        [Fact]
        public void WhenChildCommandsAreRequested_TheirAllAncestorInterceptorsAreExecuted()
        {
            // this test also proves we can NOT use the same option name for each command because they will conflict.
            // TODO: allow same name. Requires update to how ArgumentValues are keyed.
            new AppRunner<App>()
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    WhenArgs = "--name gramps Child --name2 pops GrandChild --name3 junior Greet",
                    Then =
                    {
                        Outputs =
                        {
                            "Hello, my name is junior. My parent is pops. My grandparent is gramps."
                        }
                    }
                });
        }

        [Fact]
        public void InterceptorsAreNotExecutedWhenTheirCommandIsNotInTheRequestPipeline()
        {
            // this test also proves we can NOT use the same option name for each command because they will conflict.
            // TODO: allow same name. Requires update to how ArgumentValues are keyed.
            new AppRunner<App>()
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    WhenArgs = "--name gramps GrandChild --name3 junior Greet",
                    Then =
                    {
                        Outputs =
                        {
                            "Hello, my name is junior. My parent is null. My grandparent is gramps."
                        }
                    }
                });
        }

        class App
        {
            private TestOutputs TestOutputs { get; set; }
            
            [SubCommand]
            public Child Child { get; set; }
            [SubCommand]
            public GrandChild GrandChild { get; set; }

            public string Name { get; private set; }

            public Task<int> Intercept(InterceptorExecutionDelegate next, string name)
            {
                Name = name;
                if (Child != null)
                {
                    // will be null if GrandChild is called directly or this.Greet is called
                    Child.ParentName = name;
                }
                if (GrandChild != null)
                {
                    // will be null if GrandChild is NOT called directly or this.Greet is called
                    GrandChild.GrandParentName = name;
                }
                return next();
            }

            public void Greet() => TestOutputs.Capture($"Hello, my name is {Name}. My child is {Child?.Name ?? "null"}. My grandchild is {GrandChild?.Name ?? "null"}.");
        }

        class Child
        {
            private TestOutputs TestOutputs { get; set; }

            [SubCommand]
            public GrandChild MyChild { get; set; }

            public string ParentName { get; set; }
            public string Name { get; private set; }

            public Task<int> Intercept(InterceptorExecutionDelegate next, string name2)
            {
                Name = name2;
                if (MyChild != null)
                {
                    // will be null if this.Greet is called
                    MyChild.ParentName = name2;
                    MyChild.GrandParentName = ParentName;
                }
                return next();
            }

            public void Greet() => TestOutputs.Capture($"Hello, my name is {Name}. My parent is {ParentName}. My child is {MyChild?.Name ?? "null"}");
        }

        class GrandChild
        {
            private TestOutputs TestOutputs { get; set; }

            public string GrandParentName { get; set; }
            public string ParentName { get; set; }
            public string Name { get; private set; }

            public Task<int> Intercept(InterceptorExecutionDelegate next, string name3)
            {
                Name = name3;
                return next();
            }
            
            public void Greet() => TestOutputs.Capture($"Hello, my name is {Name}. My parent is {ParentName ?? "null"}. My grandparent is {GrandParentName}.");
        }
    }
}