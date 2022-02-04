using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ClassCommands
{
    public class InheritFromBaseClassesTests
    {
        public InheritFromBaseClassesTests(ITestOutputHelper output) => Ambient.Output = output;

        [Fact]
        public void Given_InheritCommandFromBaseClasses_is_false_public_methods_from_base_classes_should_not_be_commands()
        {
            new AppRunner<App>().Verify(new Scenario
            {
                When = { Args = "-h" },
                Then =
                {
                    Output = @"Usage: testhost.dll [command]

Commands:

  Command

Use ""testhost.dll [command] --help"" for more information about a command."
                }
            });
        }

        [Fact]
        public void Given_InheritCommandFromBaseClasses_is_true_public_methods_from_base_classes_should_be_commands()
        {
            AppSettings appSettings = new AppSettings() {Commands = {InheritCommandsFromBaseClasses = true}};
            new AppRunner<App>(appSettings).Verify(new Scenario
            {
                When = { Args = "-h" },
                Then =
                {
                    Output = @"Usage: testhost.dll [command]

Commands:

  BaseCommand
  BasierCommand
  Command

Use ""testhost.dll [command] --help"" for more information about a command."
                }
            });
        }

        public class App : BaseClass
        {
            public void Command() { }
            protected void ProtectedCommand() { }
            internal void InternalCommand() { }
            protected internal void ProtectedInternalCommand() { }
        }

        public class BaseClass : BasierClass
        {
            public void BaseCommand() { }
            protected void BaseProtectedCommand() { }
            internal void BaseInternalCommand() { }
            protected internal void BaseProtectedInternalCommand() { }
        }

        public class BasierClass
        {
            public void BasierCommand() { }
            protected void BasierProtectedCommand() { }
            internal void BasierInternalCommand() { }
            protected internal void BasierProtectedInternalCommand() { }
        }
    }
}