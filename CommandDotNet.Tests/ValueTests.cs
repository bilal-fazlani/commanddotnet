using System.Collections.Generic;
using System.IO;
using CommandDotNet.Attributes;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests
{
    public class ValueTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ValueTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Theory]
        [InlineData("TestMethodParams")]
        [InlineData("TestConstructorParams")]
        public void ReadValueTest(string testCaseName)
        {
            TestCaseRunner<MyTestApp> testCaseRunner = new TestCaseRunner<MyTestApp>(_testOutputHelper);
            testCaseRunner.Run($"TestCases/MyTestApp.{testCaseName}.Input.json", 
                $"TestCases/MyTestApp.{testCaseName}.Output.json");
        }
    }

    
    public class MyTestApp
    {
        private readonly string _justAnotherParameter1;
        private readonly int _justAnotherParameter2;

        public MyTestApp(string justAnotherParameter1, [Argument(ShortName = "j")]int justAnotherParameter2)
        {
            _justAnotherParameter1 = justAnotherParameter1;
            _justAnotherParameter2 = justAnotherParameter2;
        }
        
        public void TestMethodParams(
            int id,
            [Argument(LongName = "tag", ShortName = "t")]
            List<string> tags,
            string name,
            [Argument(Flag = true)]
            bool email,
            double height,
            string city,
            bool nri,
            [Argument(LongName = "role")]
            List<int> roles,
            [Argument(LongName = "rank")]
            List<double> ranks
            )
        {
            var output = new
            {
                id,
                tags,
                name,
                email,
                height,
                city,
                nri,
                roles,
                ranks
            };
            
            File.WriteAllText("TestCases/MyTestApp.TestMethodParams.Output.json" ,JsonConvert.SerializeObject(output, Formatting.Indented));
        }

        public void TestConstructorParams(bool flag)
        {
            var output = new
            {
                justAnotherParameter1 = _justAnotherParameter1,
                flag,
                justAnotherParameter2 = _justAnotherParameter2
            };
            
            File.WriteAllText("TestCases/MyTestApp.TestConstructorParams.Output.json" , JsonConvert.SerializeObject(output, Formatting.Indented));
        }
    }
}