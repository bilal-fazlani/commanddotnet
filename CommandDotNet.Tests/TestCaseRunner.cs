using System;
using System.IO;
using CommandDotNet.Models;
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
        private readonly AppSettings _appSettings;
        
        public TestCaseRunner(ITestOutputHelper testOutputHelper, AppSettings appSettings = null)
        {
            _testOutputHelper = testOutputHelper;
            _appSettings = appSettings ?? new AppSettings();
        }
        
        public void Run(string inputFileName, string outputFileName)
        {
            TestCaseCollection testCaseCollection = JsonConvert.DeserializeObject<TestCaseCollection>(File.ReadAllText(inputFileName));

            _testOutputHelper?.WriteLine($"file: '{inputFileName}' found with {testCaseCollection.TestCases.Length} test cases");
        
            foreach (var testCase in testCaseCollection.TestCases)
            {
                _testOutputHelper?.WriteLine($"\n\n\nRunning test case : '{testCase.TestCaseName}' with params: " +
                                             $"{string.Join(", ", testCase.Params)}");
                
                

                var appRunner = new AppRunner<T>(_appSettings);
                int exitCode = appRunner.Run(testCase.Params);
                
                exitCode.Should().Be(testCase.ExpectedExitCode, $"app should return {testCase.ExpectedExitCode} exit code");

                if (testCase.ValidateOutputJson)
                {
                    JsonDiffPatch jsonDiffPatch = new JsonDiffPatch();
                    
                    var diff = jsonDiffPatch.Diff(testCase.ExpectedOutput.ToString(), File.ReadAllText(outputFileName));

                    _testOutputHelper?.WriteLine(diff != null ? $"diff found : {diff}" : "no diff found");

                    diff.Should().BeNull();

                    _testOutputHelper?.WriteLine($"test case '{testCase.TestCaseName}' passed");    
                }
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
        
        public int ExpectedExitCode { get; set; }

        public bool ValidateOutputJson { get; set; } = true;

        public override string ToString()
        {
            return  $"{TestCaseName} -> {string.Join(", ", Params)} \n {ExpectedOutput.ToJson()}";
        }
    }
}