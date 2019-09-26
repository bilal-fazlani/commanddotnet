using CommandDotNet.FluentValidation;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using FluentValidation;
using FluentValidation.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
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
            var resolver = new TestDependencyResolver{new PersonValidator()};

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
                .UseDependencyResolver(resolver)
                .VerifyScenario(TestOutputHelper, scenario);
        }

        [Fact]
        public void Exec_WithValidData_Succeeds()
        {
            var resolver = new TestDependencyResolver { new PersonValidator() };

            var scenario = new Scenario
            {
                WhenArgs = "Save 1 john john@doe.com",
                Then = {Outputs = {new Person {Id = 1, Name = "john", Email = "john@doe.com"}}}
            };

            new AppRunner<App>()
                .UseFluentValidation()
                .UseDependencyResolver(resolver)
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
                .UseDependencyResolver(new TestDependencyResolver())
                .VerifyScenario(TestOutputHelper, scenario);
        }

        public class App
        {
            public TestOutputs TestOutputs { get; set; }

            public void Save(Person person)
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
    }
}