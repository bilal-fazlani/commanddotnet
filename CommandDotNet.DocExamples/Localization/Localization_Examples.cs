using System;
using CommandDotNet;

namespace CommandDotNet.DocExamples.Localization;

public static class Localization_Examples
{
    public class ValidationApp
    {
        public void Process(string input) { }
    }

    // begin-snippet: localization_basic_setup
    public static int BasicLocalizationSetup(string[] args)
    {
        // Simple localization function - in real apps, this would call IStringLocalizer
        Func<string, string?> localizer = text => 
        {
            // Your localization logic here
            // e.g., stringLocalizer[text]
            return text; // Fallback to original text
        };
        
        var settings = new AppSettings
        {
            Localization = { Localize = localizer }
        };
        
        return new AppRunner<ValidationApp>(settings).Run(args);
    }
    // end-snippet

    // begin-snippet: localization_resources_proxy
    public static int UsingResourcesProxy(string[] args)
    {
        // Different localizers for different packages
        Func<string, string?> coreLocalizer = text => text;      // Core framework
        Func<string, string?> validationLocalizer = text => text; // Validation messages
        
        return new AppRunner<ValidationApp>(
                new AppSettings(), 
                new ResourcesProxy(coreLocalizer))
            // .UseDataAnnotationValidations(
            //     new DataAnnotations.ResourcesProxy(validationLocalizer))
            .Run(args);
    }
    // end-snippet

    // begin-snippet: localization_custom_resources
    // Custom Resources class to override specific messages
    public class MyResources : Resources
    {
        public override string Error_File_not_found(string fullPath) => 
            $"Archivo no encontrado: {fullPath}"; // Spanish: "File not found"
        
        public override string Command_help => "ayuda"; // Spanish: "help"
        
        public override string Help_Usage => "Uso"; // Spanish: "Usage"
    }

    public static int UsingCustomResources(string[] args)
    {
        return new AppRunner<ValidationApp>(
            new AppSettings(), 
            new MyResources()).Run(args);
    }
    // end-snippet

    // begin-snippet: localization_member_names_as_keys
    public static int UsingMemberNamesAsKeys(string[] args)
    {
        Func<string, string?> localizer = key => 
        {
            // When UseMemberNamesAsKeys = true, 
            // key will be "Common_argument_lc" instead of "argument"
            return key; // Look up by member name
        };
        
        var settings = new AppSettings
        {
            Localization = 
            { 
                Localize = localizer,
                UseMemberNamesAsKeys = true  // Use property names as keys
            }
        };
        
        return new AppRunner<ValidationApp>(settings).Run(args);
    }
    // end-snippet
}
