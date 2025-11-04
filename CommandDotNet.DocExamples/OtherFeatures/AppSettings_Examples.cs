using CommandDotNet.TestTools;
using FluentAssertions;
using NUnit.Framework;

namespace CommandDotNet.DocExamples.OtherFeatures;

[TestFixture]
public class AppSettings_Examples
{
    public class Program
    {
        public void SomeCommand() { }
    }

    // begin-snippet: appsettings_all_arguments_as_options
    private static AppSettings AllArgumentsAsOptions = new AppSettings
    {
        Arguments = { DefaultArgumentMode = ArgumentMode.Option }
    };
    // end-snippet

    // begin-snippet: appsettings_explicit_boolean_values
    private static AppSettings ExplicitBooleanValues = new AppSettings
    {
        Arguments = { BooleanMode = BooleanMode.Explicit }
    };
    // end-snippet

    // begin-snippet: appsettings_windows_style_options
    private static AppSettings WindowsStyleOptions = new AppSettings
    {
        Parser = { AllowBackslashOptionPrefix = true }
    };
    // end-snippet

    // begin-snippet: appsettings_powershell_style_options
    private static AppSettings PowerShellStyleOptions = new AppSettings
    {
        Parser = { AllowSingleHyphenForLongNames = true }
    };
    // end-snippet

    // begin-snippet: appsettings_expanded_help
    private static AppSettings ExpandedHelp = new AppSettings
    {
        Help = 
        { 
            ExpandArgumentsInUsage = true,
            PrintHelpOption = true
        }
    };
    // end-snippet

    // begin-snippet: appsettings_disable_directives
    private static AppSettings DisableDirectives = new AppSettings
    {
        DisableDirectives = true
    };
    // end-snippet

    // begin-snippet: appsettings_configure_method
    private static AppRunner ConfigureMethod()
    {
        return new AppRunner<Program>()
            .Configure(b => b.AppSettings.Help.PrintHelpOption = true);
    }
    // end-snippet

    // begin-snippet: appsettings_constructor
    private static AppRunner ConstructorMethod()
    {
        return new AppRunner<Program>(new AppSettings
        {
            Help = { PrintHelpOption = true }
        });
    }
    // end-snippet

    [Test] public void Obligatory_test_since_snippets_cover_all_cases() => true.Should().BeTrue();
}
