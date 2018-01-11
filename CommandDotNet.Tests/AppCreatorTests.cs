using CommandDotNet.Attributes;
using CommandDotNet.MicrosoftCommandLineUtils;
using CommandDotNet.Models;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests
{
    public class AppCreatorTests : TestBase
    {
        public AppCreatorTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void CanCreateApps()
        {
            AppCreator appCreator = new AppCreator(new AppSettings());
            CommandLineApplication app = appCreator.CreateApplication(typeof(AppCreatorTestApp), null, new CommandLineApplication());

            app.Commands.Should().HaveCount(3);

            CommandLineApplication app1 = app.Commands[1];
            app1.Name.Should().Be("MyInnerCommand");
            app1.Options.Should().HaveCount(1).And.Subject.Should().Contain(opt => opt.LongName == "help");
            app1.Arguments.Should().HaveCount(0);
            app1.Commands.Should().HaveCount(0);
            
            CommandLineApplication app2 = app.Commands[2];
            app2.Name.Should().Be("MyOtherInnerCommand");
            app2.Commands.Should().HaveCount(1);
            app2.Commands[0].Name.Should().Be("GrandChildCommand");
            app2.Options.Should().HaveCount(1).And.Subject.Should().Contain(opt => opt.LongName == "help");;
            app2.Arguments.Should().HaveCount(0);
            
            CommandLineApplication app3 = app.Commands[0];
            app3.Name.Should().Be("SendEmail");
            app3.Arguments.Should().HaveCount(1);
            app3.Arguments.Should().Contain(arg => arg.Name == "email");
            app3.Options.Should().HaveCount(2);
            app3.Options.Should().Contain(opt => opt.LongName == "name").And.Subject.Should().Contain(opt => opt.LongName == "help");
            app3.Commands.Should().HaveCount(0);
        }
    }

    public class AppCreatorTestApp
    {
        public AppCreatorTestApp(int id)
        {
            
        }
        
        private class MyInnerCommand
        {
            
        }
        
        public class MyOtherInnerCommand
        {
            public class GrandChildCommand
            {
                
            }
        }

        public void SendEmail(string email, [Option]string name)
        {
            
        }
    }
}