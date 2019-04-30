using System.IO;
using CommandDotNet.Attributes;
using CommandDotNet.Models;
using FluentAssertions;
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

        [Fact]
        public void CanConstructorsReadModels()
        {
            AppRunner<ModelAppWithConstructor> appRunner = new AppRunner<ModelAppWithConstructor>(new AppSettings()
            {
                Case = Case.KebabCase
            });
            appRunner.Run("--id", "9", "read-person-data").Should().Be(6);
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

    public class ModelAppWithConstructor
    {
        private readonly Person _person;

        public ModelAppWithConstructor(Person person)
        {
            _person = person;
        }

        public int ReadPersonData()
        {
            if (_person != null && _person.Id == 9)
                return 6;
            return 1;
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