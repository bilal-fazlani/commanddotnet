using System.Collections.Generic;
using System.Threading.Tasks;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class SeparatedArgumentTests
    {
        private readonly ITestOutputHelper _output;

        public SeparatedArgumentTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ParamsApp_NoSep_Command_DoesNotHandle_SeparatedArguments()
        {
            Assert_HandlesSeparatedArguments<ParamsApp>(ParamsApp.NoSeparator, false);
        }

        [Fact]
        public void ParamsApp_SepOnly_Command_DoesHandle_SeparatedArguments()
        {
            Assert_HandlesSeparatedArguments<ParamsApp>(ParamsApp.SeparatorOnly, true);
        }

        [Fact]
        public void ParamsApp_SepAndArgList_Command_DoesHandle_SeparatedArguments()
        {
            Assert_HandlesSeparatedArguments<ParamsApp>(ParamsApp.SeparatorAndArgList, true);
        }

        [Fact]
        public void InterceptorApp_NoSep_Command_DoesNotHandle_SeparatedArguments()
        {
            Assert_HandlesSeparatedArguments<InterceptorApp>(InterceptorApp.NoSeparator, true);
        }

        [Fact]
        public void InterceptorApp_SepAndSep_Command_DoesHandle_SeparatedArguments()
        {
            Assert_HandlesSeparatedArguments<InterceptorApp>(InterceptorApp.SeparatorAndSeparator, true);
        }

        [Fact]
        public void InterceptorApp_SepAndArgList_Command_DoesHandle_SeparatedArguments()
        {
            Assert_HandlesSeparatedArguments<InterceptorApp>(InterceptorApp.SeparatorAndArgList, true);
        }

        [Fact]
        public void ModelApp_SepOnly_Command_DoesHandle_SeparatedArguments()
        {
            Assert_HandlesSeparatedArguments<ModelApp>(ModelApp.SeparatorOnly, true);
        }

        private void Assert_HandlesSeparatedArguments<T>(string args, bool expectation) where T : class
        {
            var command = new AppRunner<T>().GetFromContext(
                args,
                _output,
                ctx => ctx.ParseResult.TargetCommand);
            command.HandlesSeparatedArguments.Should().Be(expectation);
        }

        [Fact]
        public void Given_HandlesSeparatedArguments_IndicateIn_Help_Usage()
        {
            new AppRunner<ParamsApp>()
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = "SepOnly -h",
                    Then =
                    {
                        Result = $"Usage: dotnet testhost.dll SepOnly [[--] <arg>...]"
                    }
                });
        }

        [Fact]
        public void SepOnly_Exec()
        {
            new AppRunner<ParamsApp>()
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = "SepOnly -- some separated args",
                    Then =
                    {
                        Outputs = {new[] {"some", "separated", "args"}}
                    }
                });
        }

        [Fact]
        public void ParamsApp_NoSeparator_Exec_Ignores_SeparatorArgs()
        {
            Assert_Exec<ParamsApp>(ParamsApp.NoSeparator, withSeparatedArgs:true, expectSeparatedArgs: false);
        }

        [Fact]
        public void ParamsApp_SeparatorOnly_Exec_Populates_SeparatorArgs()
        {
            Assert_Exec<ParamsApp>(ParamsApp.SeparatorOnly, true);
        }

        [Fact]
        public void ParamsApp_SepAndArgList_Exec_WithListArgs()
        {
            Assert_Exec<ParamsApp>(ParamsApp.SeparatorAndArgList, true, true);
        }

        [Fact]
        public void ParamsApp_SepAndArgList_Exec_WithoutListArgs()
        {
            Assert_Exec<ParamsApp>(ParamsApp.SeparatorAndArgList, true, false);
        }

        [Fact]
        public void InterceptorApp_NoSeparator_Exec_Populates_InterceptorSeparatorArgs()
        {
            Assert_Exec<InterceptorApp>(InterceptorApp.NoSeparator, withSeparatedArgs: true);
        }

        [Fact]
        public void InterceptorApp_SeparatorAndSeparator_Exec_Populates_BothSeparatorArgs()
        {
            Assert_Exec<InterceptorApp>(InterceptorApp.SeparatorAndSeparator, true);
        }

        [Fact]
        public void InterceptorApp_SepAndArgList_Exec_WithListArgs()
        {
            Assert_Exec<InterceptorApp>(InterceptorApp.SeparatorAndArgList, true, true);
        }

        [Fact]
        public void InterceptorApp_SepAndArgList_Exec_WithoutListArgs()
        {
            Assert_Exec<InterceptorApp>(InterceptorApp.SeparatorAndArgList, true, false);
        }

        [Fact]
        public void ModelApp_SeparatorOnly_Exec_Populates_SeparatorArgs()
        {
            Assert_Exec<ModelApp>(ModelApp.SeparatorOnly, true);
        }

        private void Assert_Exec<T>(string cmd, 
            bool withSeparatedArgs = true, 
            bool withListArgs = false,
            bool? expectSeparatedArgs = null,
            bool? expectListArgs = null) where T : class
        {
            var listArgsString = withListArgs ? " expected args" : "";
            var separatedArgsString = withSeparatedArgs ? " -- some separated args" : "";
            
            var outputs = new List<object>();
            if (expectListArgs ?? withListArgs)
            {
                outputs.Add(new List<string> {"expected", "args"});
            }
            if (expectSeparatedArgs ?? withSeparatedArgs)
            {
                outputs.Add(new[] {"some", "separated", "args"});
            }

            new AppRunner<T>()
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = $"{cmd}{listArgsString}{separatedArgsString}",
                    Then =
                    {
                        Outputs = outputs
                    }
                });
        }

        [Fact]
        public void CanBeEnumerable()
        {
            new AppRunner<CollectionTypesApp>()
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = "Enumerable -- some separated args",
                    Then =
                    {
                        Outputs = {new [] {"some", "separated", "args"}}
                    }
                });
        }

        [Fact]
        public void CannotBeListInParam()
        {
            new AppRunner<InvalidListParameterCollectionTypesApp>()
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = "List -- some separated args",
                    Then =
                    {
                        ExitCode = 1,
                        Result = "Parameter 'InvalidListParameterCollectionTypesApp.List.separated' attributed with SeparatedArgumentsAttribute must be assignable from string[]"
                    }
                });
        }

        [Fact]
        public void CannotBeListInModel()
        {
            new AppRunner<InvalidListModelCollectionTypesApp>()
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = "Model -- some separated args",
                    Then =
                    {
                        ExitCode = 1,
                        Result = "Property 'InvalidListModelCollectionTypesApp.Model.Separated' attributed with SeparatedArgumentsAttribute must be assignable from string[]"
                    }
                });
        }

        public class ParamsApp
        {
            public const string NoSeparator = nameof(NoSep);
            public const string SeparatorOnly = nameof(SepOnly);
            public const string SeparatorAndArgList = nameof(SepAndArgList);

            private TestOutputs TestOutputs { get; set; }

            public void NoSep()
            {
            }

            public void SepOnly([SeparatedArguments] string[] separated)
            {
                TestOutputs.CaptureIfNotNull(separated);
            }

            public void SepAndArgList(List<string> args, [SeparatedArguments] string[] separated)
            {
                TestOutputs.CaptureIfNotNull(args);
                TestOutputs.CaptureIfNotNull(separated);
            }
        }

        public class InterceptorApp
        {
            public const string NoSeparator = nameof(NoSep);
            public const string SeparatorAndSeparator = nameof(SepAndSep);
            public const string SeparatorAndArgList = nameof(ArgList);

            private TestOutputs TestOutputs { get; set; }

            public Task<int> Interceptor(InterceptorExecutionDelegate next, [SeparatedArguments] string[] separated)
            {
                TestOutputs.CaptureIfNotNull(separated);
                return next();
            }

            public void NoSep()
            {
            }

            public void SepAndSep([SeparatedArguments] string[] separated)
            {
                separated.Should().BeEquivalentTo(TestOutputs.Get<string[]>());
            }

            public void ArgList(List<string> args)
            {
                TestOutputs.CaptureIfNotNull(args);
            }
        }

        public class ModelApp
        {
            public const string SeparatorOnly = "SepOnly";

            private TestOutputs TestOutputs { get; set; }

            public void SepOnly(Model model)
            {
                TestOutputs.CaptureIfNotNull(model.Separated);
            }

            public class Model : IArgumentModel
            {
                [SeparatedArguments]
                public string[] Separated { get; set; }
            }
        }

        public class CollectionTypesApp
        {
            private TestOutputs TestOutputs { get; set; }

            public void Enumerable([SeparatedArguments] IEnumerable<string> separated)
            {
                TestOutputs.CaptureIfNotNull(separated);
            }
        }

        public class InvalidListParameterCollectionTypesApp
        {
            private TestOutputs TestOutputs { get; set; }

            public void List([SeparatedArguments] List<string> separated)
            {
                TestOutputs.CaptureIfNotNull(separated);
            }
        }

        public class InvalidListModelCollectionTypesApp
        {
            private TestOutputs TestOutputs { get; set; }

            public void List(Model model)
            {
                TestOutputs.CaptureIfNotNull(model.Separated);
            }

            public class Model : IArgumentModel
            {
                [SeparatedArguments]
                public List<string> Separated { get; set; }
            }
        }
    }
}