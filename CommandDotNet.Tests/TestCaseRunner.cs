using System.IO;
using FluentAssertions;
using JsonDiffPatchDotNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit.Abstractions;

namespace CommandDotNet.Tests
{
    public class TestCaseRunner<T> where T: class 
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public TestCaseRunner(ITestOutputHelper testOutputHelper = null)
        {
            _testOutputHelper = testOutputHelper;
        }
        
        public void Run(string inputFileName, string outputFileName)
        {            
            TestCaseCollection testCaseCollection = JsonConvert.DeserializeObject<TestCaseCollection>(File.ReadAllText(inputFileName));

            _testOutputHelper?.WriteLine($"file: '{inputFileName}' found with {testCaseCollection.TestCases.Length} test cases");
            
            foreach (var testCase in testCaseCollection.TestCases)
            {
                _testOutputHelper?.WriteLine($"\n\n\nRunning test case : '{testCase.TestCaseName}'");
                
                AppRunner<T> appRunner = new AppRunner<T>();

                int exitCode = appRunner.Run(testCase.Params);

                exitCode.Should().Be(0);

                JsonDiffPatch jsonDiffPatch = new JsonDiffPatch();

                var diff = jsonDiffPatch.Diff(testCase.ExpectedOutput.ToString(), File.ReadAllText(outputFileName));

                if(diff != null) _testOutputHelper?.WriteLine($"diff found : {diff}");
                else _testOutputHelper?.WriteLine("no diff found");
                
                diff.Should().BeNull();

                _testOutputHelper?.WriteLine($"test case '{testCase.TestCaseName}' passed");
            }
        }
    }

    public class TestCaseCollection
    {
        public TestCase[] TestCases { get; set; }
    }
    
    public class TestCase
    {
        public string TestCaseName { get; set; }
        
        public string[] Params { get; set; }
        
        public JObject ExpectedOutput { get; set; }
    }
}