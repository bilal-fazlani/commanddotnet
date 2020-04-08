using CommandDotNet.Execution;
using CommandDotNet.FluentValidation;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using FluentValidation;
using FluentValidation.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.CommandDotNet.FluentValidation
{
    public class ModelValidationTests : TestBase
    {
        public ModelValidationTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Help_DoesNotIncludeValidation()
        {
            var scenario = new Scenario
            {
                WhenArgs = "Save -h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll Save [arguments]

Arguments:

  Id     <NUMBER>      

  Name   <TEXT>        

  Email  <TEXT>"
                }
            };

            new AppRunner<App>()
                .UseFluentValidation()
                .VerifyScenario(TestOutputHelper, scenario);
        }

        [Fact]
        public void Exec_WithInvalidData_PrintsValidationError()
        {
            var scenario = new Scenario
            {
                WhenArgs = "Save",
                Then =
                {
                    ExitCode = 2,
                    Result = @"'Person' is invalid
  'Id' must be greater than '0'.
  'Name' should not be empty.
  'Email' should not be empty."
                }
            };

            new AppRunner<App>()
                .UseFluentValidation()
                .VerifyScenario(TestOutputHelper, scenario);
        }

        [Fact]
        public void Exec_WithInvalidData_PrintsValidationError_UsingValidatorFromDI()
        {
            var scenario = new Scenario
            {
                WhenArgs = "Save",
                Then =
                {
                    ExitCode = 2,
                    Result = @"'Person' is invalid
  'Id' must be greater than '0'.
  'Name' should not be empty.
  'Email' should not be empty."
                }
            };

            new AppRunner<App>()
                .UseFluentValidation()
                .UseDependencyResolver(
                    new TestDependencyResolver{new PersonValidator()}, 
                    commandClassResolveStrategy: ResolveStrategy.TryResolve)
                .VerifyScenario(TestOutputHelper, scenario);
        }

        [Fact]
        public void Exec_WithValidData_Succeeds()
        {
            var scenario = new Scenario
            {
                WhenArgs = "Save 1 john john@doe.com",
                Then = {Outputs = {new Person {Id = 1, Name = "john", Email = "john@doe.com"}}}
            };

            new AppRunner<App>()
                .UseFluentValidation()
                .UseDependencyResolver(
                    new TestDependencyResolver { new PersonValidator() },
                    commandClassResolveStrategy: ResolveStrategy.TryResolve)
                .VerifyScenario(TestOutputHelper, scenario);
        }

        [Fact]
        public void Exec_WhenNoDependencyResolver_ValidatorIsCreated()
        {
            var scenario = new Scenario
            {
                WhenArgs = "Save",
                Then =
                {
                    ExitCode = 2,
                    ResultsContainsTexts = {"'Person' is invalid"}
                }
            };

            new AppRunner<App>()
                .UseFluentValidation()
                .VerifyScenario(TestOutputHelper, scenario);
        }

        [Fact]
        public void Exec_WhenValidatorNotRegistered_ValidatorIsCreated()
        {
            var scenario = new Scenario
            {
                WhenArgs = "Save",
                Then =
                {
                    ExitCode = 2,
                    ResultsContainsTexts = {"'Person' is invalid"}
                }
            };

            new AppRunner<App>()
                .UseFluentValidation()
                .UseDependencyResolver(
                    new TestDependencyResolver(),
                    commandClassResolveStrategy: ResolveStrategy.TryResolve)
                .VerifyScenario(TestOutputHelper, scenario);
        }

        [Fact]
        public void Exec_WhenInvalidValidator_PrintsError()
        {
            var scenario = new Scenario
            {
                WhenArgs = "InvalidSave",
                Then =
                {
                    ExitCode = 1,
                    ResultsContainsTexts =
                    {
                        @"CommandDotNet.FluentValidation.InvalidValidatorException: Could not create instance of InvalidPersonValidator. Please ensure it's injected via IoC or has a default constructor.
This exception could also occur if default constructor threw an exception",
                        " ---> System.MissingMethodException: No parameterless constructor defined for this object" // assert stack trace is printed
                    }
                }
            };

            new AppRunner<App>()
                .UseFluentValidation()
                .UseDependencyResolver(
                    new TestDependencyResolver(),
                    commandClassResolveStrategy: ResolveStrategy.TryResolve)
                .VerifyScenario(TestOutputHelper, scenario);
        }

        [Fact]
        public void Exec_IfShowHelpOnError_ShowsHelpAfterValidationMessage()
        {
            var scenario = new Scenario
            {
                WhenArgs = "Save",
                Then =
                {
                    ExitCode = 2,
                    ResultsContainsTexts =
                    {
                        @"'Person' is invalid
  'Id' must be greater than '0'.
  'Name' should not be empty.
  'Email' should not be empty.

Usage: dotnet testhost.dll Save [arguments]"
                    }
                }
            };

            new AppRunner<App>()
                .UseFluentValidation(showHelpOnError: true)
                .VerifyScenario(TestOutputHelper, scenario);
        }

        public class App
        {
            public TestOutputs TestOutputs { get; set; }

            public void Save(Person person)
            {
                TestOutputs.Capture(person);
            }

            public void InvalidSave(InvalidPerson person)
            {
                TestOutputs.Capture(person);
            }
        }

        [Validator(typeof(PersonValidator))]
        public class Person : IArgumentModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
        }

        public class PersonValidator : AbstractValidator<Person>
        {
            public PersonValidator()
            {
                RuleFor(x => x.Id).GreaterThan(0);
                RuleFor(x => x.Name).NotEmpty();
                RuleFor(x => x.Email).NotEmpty().EmailAddress();
            }
        }

        [Validator(typeof(InvalidPersonValidator))]
        public class InvalidPerson : IArgumentModel
        {
            public int Id { get; set; }
        }

        public class InvalidPersonValidator : AbstractValidator<Person>
        {
            public InvalidPersonValidator(bool nonDefaultCtor)
            {
                RuleFor(x => x.Id).GreaterThan(0);
                RuleFor(x => x.Name).NotEmpty();
                RuleFor(x => x.Email).NotEmpty().EmailAddress();
            }
        }
    }
}