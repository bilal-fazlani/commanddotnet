using System.Collections.Generic;
using System.IO;
using CommandDotNet.Attributes;
using CommandDotNet.Models;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests
{
    public class ValueTests: TestBase
    {
        public ValueTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Theory]
        [InlineData("TestMethodParams")]
        [InlineData("TestConstructorParams")]
        public void ReadValueTest(string testCaseName)
        {
            TestCaseRunner<MyTestApp> testCaseRunner = new TestCaseRunner<MyTestApp>(TestOutputHelper, new AppSettings
            {
                MethodArgumentMode = ArgumentMode.Option
            });
            testCaseRunner.Run($"TestCases/MyTestApp.{testCaseName}.Input.json", 
                $"TestCases/MyTestApp.{testCaseName}.Output.json");
        }
    }

    
    public class MyTestApp
    {
        private readonly string _justAnotherParameter;

        public MyTestApp(string justAnotherParameter)
        {
            _justAnotherParameter = justAnotherParameter;
        }
        
        public void TestMethodParams(
            int id,
            [Option(LongName = "tag", ShortName = "t")]
            List<string> tags,
            string name,
            [Option(BooleanMode = BooleanMode.Explicit)]
            bool email,
            bool sms,
            double height,
            string city,
            char category,
            bool nri,
            [Option(LongName = "role")]
            List<int> roles,
            [Option(LongName = "rank")]
            List<double> ranks
            )
        {
            var output = new
            {
                id,
                tags,
                name,
                email,
                sms,
                height,
                city,
                category,
                nri,
                roles,
                ranks
            };
            
            File.WriteAllText("TestCases/MyTestApp.TestMethodParams.Output.json", JsonConvert.SerializeObject(output, Formatting.Indented));
        }

        public void TestConstructorParams()
        {
            var output = new
            {
                justAnotherParameter = _justAnotherParameter,
            };
            
            File.WriteAllText("TestCases/MyTestApp.TestConstructorParams.Output.json", JsonConvert.SerializeObject(output, Formatting.Indented));
        }
    }
}