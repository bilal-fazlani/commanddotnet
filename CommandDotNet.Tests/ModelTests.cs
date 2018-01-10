using System;
using System.IO;
using System.Runtime.CompilerServices;
using CommandDotNet.Attributes;
using CommandDotNet.Models;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
        public void CanReadModels()
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
        public int SendNotification(Person person, Time time, Address address)
        {
            object obj = new
            {
                person,
                time,
                address
            };
            
            string content = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            });
            
            File.WriteAllText("TestCases/ModelTests.ModelTests.Output.json", content);
            return 5;
        }
    }
    

    public class Person: IArgumentModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Option]
        public string Email { get; set; }
        
        public string Number { get; set; }
    }

    public class Address : IArgumentModel
    {
        public string City { get; set; }
        
        [Option(ShortName = "a")]
        public bool HasAirport { get; set; }
    }
}