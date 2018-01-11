using CommandDotNet.Models;
using FluentValidation;
using FluentValidation.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests
{
    public class ValidationTests : TestBase
    {
        public ValidationTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void CanValidateModel()
        {
            TestCaseRunner<ValidationApp> testCaseRunner = new TestCaseRunner<ValidationApp>(TestOutputHelper, new AppSettings()
            {
                Case = Case.LowerCase
            });
            
            testCaseRunner.Run("TestCases/ValidationTests.CanValidateModel.Input.json", "TestCases/ValidationTests.CanValidateModel.Output.json");
        }
    }

    public class ValidationApp
    {
        public void ValidateModel(PersonModel person)
        {
            person.ToJson().WriteToFile("TestCases/ValidationTests.CanValidateModel.Output.json");
        }
    }

    [Validator(typeof(PersonValidator))]
    public class PersonModel : IArgumentModel
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public string Email { get; set; }
    }

    public class PersonValidator : AbstractValidator<PersonModel>
    {
        public PersonValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }
    }
}