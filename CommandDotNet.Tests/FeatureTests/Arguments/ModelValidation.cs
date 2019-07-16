using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.Tests.Utils;
using FluentValidation;
using FluentValidation.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class ModelValidation
    {
        private readonly ScenarioVerifier _verifier;

        public ModelValidation(ITestOutputHelper output)
        {
            _verifier = new ScenarioVerifier(output);
        }

        [Fact]
        public void Help_DoesNotIncludeValidation()
        {
            var scenario = new Given<App>
            {
                WhenArgs = "Save -h",
                Then = {
                    Result = @"Usage: dotnet testhost.dll Save [arguments]

Arguments:

  Id       <NUMBER>      

  Name     <TEXT>        

  Email    <TEXT>" }
            };
            _verifier.VerifyScenario(scenario);
        }

        [Fact]
        public void Exec_WithInvalidData_PrintsValidationError()
        {
            var scenario = new Given<App>
            {
                WhenArgs = "Save",
                Then = { 
                    ExitCode = 2,
                    Result = @"'Person' is invalid
  'Id' must be greater than '0'.
  'Name' should not be empty.
  'Email' should not be empty." }
            };
            _verifier.VerifyScenario(scenario);
        }

        [Fact]
        public void Exec_WithInvalidData_PrintsValidationError_UsingValidatorFromDI()
        {
            var scenario = new Given<App>
            {
                And = { Dependencies = {new PersonValidator()}},
                WhenArgs = "Save",
                Then = {
                    ExitCode = 2,
                    Result = @"'Person' is invalid
  'Id' must be greater than '0'.
  'Name' should not be empty.
  'Email' should not be empty." }
            };
            _verifier.VerifyScenario(scenario);
        }

        [Fact]
        public void Exec_WithValidData_Succeeds()
        {
            var scenario = new Given<App>
            {
                WhenArgs = "Save 1 john john@doe.com",
                Then = { Outputs = { new Person{Id = 1, Name = "john", Email = "john@doe.com"}}}
            };
            _verifier.VerifyScenario(scenario);
        }

        public class App
        {
            [InjectProperty]
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