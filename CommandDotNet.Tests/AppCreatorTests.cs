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
            var appSettings = new AppSettings();
            AppCreator appCreator = new AppCreator(appSettings);
            CommandLineApplication app = appCreator.CreateApplication(typeof(AppCreatorTestApp), null, 
                new CommandLineApplication(appSettings));

            app.Commands.Should().HaveCount(3);

            ICommand app1 = app.Commands[1];
            app1.Name.Should().Be("MyInnerCommand");
            app1.Options.Should().HaveCount(1).And.Subject.Should().Contain(opt => opt.LongName == "help");
            app1.Arguments.Should().HaveCount(0);
            app1.Commands.Should().HaveCount(0);
            
            ICommand app2 = app.Commands[2];
            app2.Name.Should().Be("MyOtherInnerCommand");
            app2.Commands.Should().HaveCount(1);
            app2.Commands[0].Name.Should().Be("GrandChildCommand");
            app2.Options.Should().HaveCount(1).And.Subject.Should().Contain(opt => opt.LongName == "help");;
            app2.Arguments.Should().HaveCount(0);
            
            ICommand app3 = app.Commands[0];
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
        
        [SubCommand]
        private class MyInnerCommand
        {
            
        }
        
        [SubCommand]
        public class MyOtherInnerCommand
        {
            [SubCommand]
            public class GrandChildCommand
            {
                
            }
        }

        public void SendEmail(string email, [Option]string name)
        {
            
        }
    }
}