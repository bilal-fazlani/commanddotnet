using System;
using CommandDotNet.Attributes;
using CommandDotNet.Models;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests
{
    public class SubjectTests : TestBase
    {
        public SubjectTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void CanRecogniseSubject()
        {
            AppRunner<FakeDockerApp> appRunner = new AppRunner<FakeDockerApp>();
            appRunner.Run("Run", "dotnet")
                .Should().Be(5, "run command is expected to return 5 when passed argument is recognised");
        }

        [Fact]
        public void CanRecogniseSubjectWithOtherArguments()
        {
            AppRunner<FakeDockerApp> appRunner = new AppRunner<FakeDockerApp>();
            appRunner.Run("RunWithOtherParameters", "--username", "bilal", "-l", "2", "dotnet")
                .Should().Be(5, "run command is expected to return 5 when passed argument is recognised");
        }
        
        [Fact]
        public void CanRecogniseSubjectWithOtherArgumentsForDefaultMethodWithConstructor()
        {
            AppRunner<FakeDockerAppWithConstructor> appRunner = new AppRunner<FakeDockerAppWithConstructor>();
            appRunner.Run("--username", "bilal", "-l", "2", "dotnet")
                .Should().Be(5, "run command is expected to return 5 when passed argument is recognised");
        }
        
        [Fact(Skip = "Known Issue")]
        public void CanRecogniseSubjectWhenSubjectPassedAtBeginning()
        {
            AppRunner<FakeDockerApp> appRunner = new AppRunner<FakeDockerApp>();
            appRunner.Run("RunWithOtherParameters", "dotnet", "--username", "bilal", "-l", "2")
                .Should().Be(5, "run command is expected to return 5 when passed argument is recognised");
        }
        
        //todo: test with lists
        //todo: test with sub commands
    }

    public class FakeDockerApp
    {        
        public int Run([Subject] string imageName)
        {
            Console.WriteLine(JsonConvert.SerializeObject(new
            {
                testName = "Run",
                imageName
            }));
            
            imageName.Should().Be("dotnet");
            return 5;
        }
        
        public int RunWithOtherParameters(string username, [Subject] string imageName, int l)
        {
            Console.WriteLine(JsonConvert.SerializeObject(new
            {
                testName = "RunWithOtherParameters",
                username,
                imageName,
                l
            }));
            
            username.Should().Be("bilal");
            imageName.Should().Be("dotnet");
            l.Should().Be(2);
            return 5;
        }
    }

    public class FakeDockerAppWithConstructor
    {
        private readonly string _username;
        private readonly string _imageName;
        private readonly int _l;

        public FakeDockerAppWithConstructor(string username, [Subject] string imageName, int l)
        {
            Console.WriteLine(JsonConvert.SerializeObject(new
            {
                testName = "FakeDockerAppWithConstructor.Constructor",
                username,
                imageName,
                l
            }));
            
            _username = username;
            _imageName = imageName;
            _l = l;
        }
        
        [DefaultMethod]
        public int RunDefaultMethodWithSubjectAndOtherParameters()
        {
            Console.WriteLine(JsonConvert.SerializeObject(new
            {
                testName = "RunDefaultMethodWithSubjectAndOtherParameters",
                _username,
                _imageName,
                _l
            }));
            
            _username.Should().Be("bilal");
            _imageName.Should().Be("dotnet");
            _l.Should().Be(2);
            return 5;
        }
    }
}