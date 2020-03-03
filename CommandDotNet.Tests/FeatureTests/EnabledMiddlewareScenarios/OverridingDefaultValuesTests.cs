using System;
using System.Collections.Generic;
using CommandDotNet.Tests.FeatureTests.Arguments.Models;
using CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels;
using CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsParams;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.EnabledMiddlewareScenarios
{
    public class OverridingDefaultValuesTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public OverridingDefaultValuesTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void DefaultValuesShouldBeOverrideable()
        {
            var overrides = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                ["stringArg"] = "green",
                ["structArg"] = 1,
                ["structNArg"] = 2,
                ["enumArg"] = DayOfWeek.Monday,
                ["objectArg"] = new Uri("http://google.com"),
                ["stringListArg"] = new List<string> { "yellow", "orange" }
            };
            new AppRunner<App>()
                .SetDefaults(overrides)
                .VerifyScenario(_testOutputHelper,
                    new Scenario
                    {
                        WhenArgs = "ArgsDefaults",
                        Then =
                        {
                            Outputs = { new ParametersSampleTypesResults
                            {
                                BoolArg = true,
                                StringArg = "green",
                                StructArg = 1,
                                StructNArg = 2,
                                EnumArg = DayOfWeek.Monday,
                                ObjectArg = new Uri("http://google.com"),
                                StringListArg = new List<string>{"yellow", "orange"}
                            } }
                        }
                    });
            new AppRunner<App>()
                .SetDefaults(overrides)
                .VerifyScenario(_testOutputHelper,
                    new Scenario
                    {
                        WhenArgs = "OperandsDefaultsModel",
                        Then =
                        {
                            Outputs = { new OperandsDefaultsSampleTypesModel
                            {
                                BoolArg = true,
                                StringArg = "green",
                                StructArg = 1,
                                StructNArg = 2,
                                EnumArg = DayOfWeek.Monday,
                                ObjectArg = new Uri("http://google.com"),
                                StringListArg = new List<string>{"yellow", "orange"}
                            } }
                        }
                    });
        }

        [Fact]
        public void DefaultValuesCanBeParsedFromString()
        {
            var overrides = new Dictionary<string, object>()
            {
                ["stringArg"] = "green",
                ["structArg"] = 1,
                ["structNArg"] = 2,
                ["enumArg"] = "Monday",
                ["objectArg"] = "http://google.com",
                ["stringListArg"] = new List<string>{ "yellow","orange" },
                ["enumListArg"] = new List<string>{ "Monday", "Friday" },
                ["objectListArg"] = new List<string> { "http://www.google.com", "http://www.apple.com" }
            };
            new AppRunner<App>()
                .SetDefaults(overrides)
                .VerifyScenario(_testOutputHelper,
                new Scenario
                {
                    WhenArgs = "ArgsDefaults",
                    Then =
                    {
                        Outputs =
                        {
                            new ParametersSampleTypesResults
                            {
                                BoolArg = true,
                                StringArg = "green",
                                StructArg = 1,
                                StructNArg = 2,
                                EnumArg = DayOfWeek.Monday,
                                ObjectArg = new Uri("http://google.com"),
                                StringListArg = new List<string> {"yellow", "orange"}
                            }
                        }
                    }
                });
            new AppRunner<App>()
                .SetDefaults(overrides)
                .VerifyScenario(_testOutputHelper,
                new Scenario
                {
                    WhenArgs = "EnumListDefaults",
                    Then =
                    {
                        Outputs =
                        {
                            new ParametersSampleTypesResults(new List<DayOfWeek> {DayOfWeek.Monday, DayOfWeek.Friday})
                        }
                    }
                });
            new AppRunner<App>()
                .SetDefaults(overrides)
                .VerifyScenario(_testOutputHelper,
                    new Scenario
                    {
                        WhenArgs = "ObjectListDefaults",
                        Then =
                        {
                            Outputs =
                            {
                                new ParametersSampleTypesResults(new List<Uri>
                                {
                                    new Uri("http://www.google.com"),
                                    new Uri("http://www.apple.com")
                                })
                            }
                        }
                    });
        }

        [Fact]
        public void DefaultValuesCanBeCoerced()
        {
            // for cases where an int is provided but the argument type is actually a long
            var overrides = new Dictionary<string, object>
            {
                ["StructArg"] = (long)1,
            };
            new AppRunner<App>()
                .SetDefaults(overrides)
                .VerifyScenario(_testOutputHelper,
                    new Scenario
                    {
                        WhenArgs = "OptionsDefaultsModel",
                        Then =
                        {
                            Outputs = { new OptionsDefaultsSampleTypesModel
                            {
                                StructArg = 1,
                            } }
                        }
                    });
        }

        [Fact]
        public void DefaultValuesFailureIncludesArgumentInfo()
        {
            // for cases where an int is provided but the argument type is actually a long
            var overrides = new Dictionary<string, object>
            {
                ["StructNArg"] = (long)1,
            };
            new AppRunner<App>()
                .SetDefaults(overrides)
                .VerifyScenario(_testOutputHelper,
                    new Scenario
                    {
                        WhenArgs = "OptionsDefaultsModel",
                        Then =
                        {
                            ExitCode = 2,
                            ResultsContainsTexts =
                            {
                                "Failure assigning value to Option: StructNArg",
                                "CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels.OptionsDefaultsSampleTypesModel.StructNArg",
                                "Invalid cast from 'System.Int64' to 'System.Nullable`1"
                            }
                        }
                    });

        }

        class App : IArgsDefaultsSampleTypesMethod
        {
            public TestOutputs TestOutputs { get; set; }

            public void ArgsDefaults(
                [Option] bool boolArg = true,
                [Option] string stringArg = "lala",
                [Option] int structArg = 3,
                [Option] int? structNArg = 4,
                [Option] DayOfWeek enumArg = DayOfWeek.Wednesday,
                [Option] Uri objectArg = null,
                [Option] List<string> stringListArg = null)
            {
                TestOutputs.Capture(new ParametersSampleTypesResults(
                    boolArg, stringArg, structArg, structNArg, enumArg, objectArg, stringListArg));
            }

            public void StructListDefaults(
                [Option] List<int> structListArg = null)
            {
                TestOutputs.Capture(new ParametersSampleTypesResults(structListArg));
            }

            public void EnumListDefaults(
                [Option] List<DayOfWeek> enumListArg = null)
            {
                TestOutputs.Capture(new ParametersSampleTypesResults(enumListArg));
            }

            public void ObjectListDefaults(
                [Option] List<Uri> objectListArg = null)
            {
                TestOutputs.Capture(new ParametersSampleTypesResults(objectListArg));
            }

            public void OperandsDefaultsModel(OperandsDefaultsSampleTypesModel model)
            {
                TestOutputs.Capture(model);
            }

            public void OptionsDefaultsModel(OptionsDefaultsSampleTypesModel model)
            {
                TestOutputs.Capture(model);
            }
        }
    }
}