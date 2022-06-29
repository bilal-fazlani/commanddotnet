using System;
using System.Collections.Generic;
using System.Reflection;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class ArityValidationTests
    {
        private AppRunner<App> AppRunner => new(new AppSettings{Arguments = {DefaultArgumentMode = ArgumentMode.Option}});

        private AppRunner<App> AppRunner_SkipValidation => new(new AppSettings
            { Arguments = { DefaultArgumentMode = ArgumentMode.Option, SkipArityValidation = true } });

        public ArityValidationTests(ITestOutputHelper output) => Ambient.Output = output;

        [Fact(Skip = "Has to be run by itself to work. Only good for manual testing.")]
        //[Fact]
        public void Missing_required_single_ValueType_does_not_fail_when_NullabilityInfoContext_is_disabled()
        {
            try
            {
                AppContext.SetSwitch("System.Reflection.NullabilityInfoContext.IsSupported", false);
                AppRunner.Verify(new Scenario
                {
                    When = { Args = "Int" },
                    Then = { Output = "" }
                });
            }
            finally
            {
                AppContext.SetSwitch("System.Reflection.NullabilityInfoContext.IsSupported", true);
            }
        }

        [Fact]
        public void Missing_required_single_ValueType_does_not_fail_when_SkipArityValidation_is_true()
        {
            AppRunner_SkipValidation.Verify(new Scenario
            {
                When = { Args = "Int" },
                Then = { Output = "" }
            });
        }

        [Fact]
        public void Missing_required_single_ValueType_fails_with_error()
        {
            AppRunner.Verify(new Scenario
            {
                When = { Args = "Int" },
                Then = { ExitCode = 2, Output = "requiredInt is required" }
            });
        }

        [Fact]
        public void Providing_required_single_ValueType_succeeds()
        {
            AppRunner.Verify(new Scenario
            {
                When = { Args = "Int --requiredInt 1" },
                Then = { Output = "" }
            });
        }

        [Fact]
        public void Input_matches_default_of_required_single_ValueType()
        {
            AppRunner.Verify(new Scenario
            {
                When = { Args = "Int --requiredInt 0" },
                Then = { ExitCode = 0, Output = "" }
            });
        }

        [Fact]
        public void Missing_required_single_ReferenceType_fails_with_error()
        {
            AppRunner.Verify(new Scenario
            {
                When = { Args = "Uri" },
                Then = { ExitCode = 2, Output = "requiredUri is required" }
            });
        }

        [Fact]
        public void Providing_required_single_ReferenceType_succeeds()
        {
            AppRunner.Verify(new Scenario
            {
                When = { Args = "Uri --requiredUri abc.com" },
                Then = { Output = "" }
            });
        }

        [Fact]
        public void Model_Missing_required_single_ValueType_fails_with_error()
        {
            AppRunner.Verify(new Scenario
            {
                When = { Args = "IntModel" },
                Then = { ExitCode = 2, Output = "RequiredInt is required" }
            });
        }

        [Fact]
        public void Model_Providing_required_single_ValueType_succeeds()
        {
            AppRunner.Verify(new Scenario
            {
                When = { Args = "IntModel --RequiredInt 1" },
                Then = { Output = "" }
            });
        }

        [Fact]
        public void Model_input_matches_default_of_required_single_ValueType()
        {
            AppRunner.Verify(new Scenario
            {
                When = { Args = "IntModel --RequiredInt 0" },
                Then = { ExitCode = 0, Output = "" }
            });
        }

        [Fact]
        public void Model_Missing_required_single_ReferenceType_fails_with_error()
        {
            AppRunner.Verify(new Scenario
            {
                When = { Args = "UriModel" },
                Then = { ExitCode = 2, Output = "RequiredUri is required" }
            });
        }

        [Fact]
        public void Model_Providing_required_single_ReferenceType_succeeds()
        {
            AppRunner.Verify(new Scenario
            {
                When = { Args = "UriModel --RequiredUri abc.com" },
                Then = { Output = "" }
            });
        }


        [Fact]
        public void Missing_required_multi_ValueType_fails_with_error()
        {
            AppRunner.Verify(new Scenario
            {
                When = { Args = "IntList" },
                Then = { ExitCode = 2, Output = "requiredInt is required" }
            });
        }

        [Fact]
        public void Providing_required_multi_ValueType_succeeds()
        {
            AppRunner.Verify(new Scenario
            {
                When = { Args = "IntList --requiredInt 1" },
                Then = { Output = "" }
            });
        }

        [Fact]
        public void Missing_required_multi_ReferenceType_fails_with_error()
        {
            AppRunner.Verify(new Scenario
            {
                When = { Args = "UriList" },
                Then = { ExitCode = 2, Output = "requiredUri is required" }
            });
        }

        [Fact]
        public void Providing_required_multi_ReferenceType_succeeds()
        {
            AppRunner.Verify(new Scenario
            {
                When = { Args = "UriList --requiredUri abc.com" },
                Then = { Output = "" }
            });
        }

        public class App
        {
            public void Int(int requiredInt, int? nullableInt, int optionalInt = default!){ }
            public void Uri(Uri requiredUri, Uri? nullableUri, Uri optionalUri = default!) { }

            public void IntModel(ModelOfInts modelOfInts) { }
            public void UriModel(ModelOfUris modelOfUris) { }

            public void IntList(List<int> requiredInt, List<int>? nullableInt, List<int> optionalInt = default!) { }
            public void UriList(List<Uri> requiredUri, List<Uri>? nullableUri, List<Uri> optionalUri = default!) { }

            public class ModelOfInts : IArgumentModel
            {
                public int RequiredInt { get; set; }
                public int? NullableInt { get; set; }
                public int DefaultInt { get; set; } = 1;
            }

            public class ModelOfUris : IArgumentModel
            {
                public Uri RequiredUri { get; set; } = null!;
                public Uri? NullableUri { get; set; }
                public Uri DefaultUri { get; set; } = new("http://google.com");
            }
        }
    }
}