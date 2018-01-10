using System;
using System.IO;
using System.Runtime.CompilerServices;
using CommandDotNet.Attributes;
using CommandDotNet.Models;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Attributes;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests
{
    public class ModelTests : TestBase
    {
        public ModelTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void CanReadAndValidateModels()
        {
            TestCaseRunner<ModelApp> testCaseRunner = new TestCaseRunner<ModelApp>(TestOutputHelper, new AppSettings
            {
                Case = Case.KebabCase
            });
            
            testCaseRunner.Run(
                inputFileName: "TestCases/ModelTests.ModelTests.Input.json", 
                outputFileName: "TestCases/ModelTests.ModelTests.Output.json");
        }
    }

    public class ModelApp
    {
        public int ProcessModel(ProcessParams values)
        {
            string content = JsonConvert.SerializeObject(values, Formatting.Indented);
            
            File.WriteAllText("TestCases/ModelTests.ModelTests.Output.json", content);
            return 5;
        }
    }
    
    [Validator(typeof(ProcessModelValidator))]
    public class ProcessParams: IArgumentModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Option]
        public string Email { get; set; }
    }

    public class ProcessModelValidator : AbstractValidator<ProcessParams>
    {
        public ProcessModelValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }
}