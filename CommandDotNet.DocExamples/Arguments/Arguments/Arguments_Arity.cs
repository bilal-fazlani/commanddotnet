using System;
using NUnit.Framework;

namespace CommandDotNet.DocExamples.Arguments.Arguments
{ 
// Cannot convert null literal to non-nullable reference type.
// These are intentional for the examples for cases where NRTs are not enabled.
#pragma warning disable CS8625
#pragma warning disable CS8618
    
    [TestFixture]
    public class Arguments_Arity
    {
        public class Program
        {
            [DefaultCommand]
            // begin-snippet: arguments_arity
            public void DefaultCommand(Model model,
                    bool requiredBool, Uri requiredRefType, 
                    bool? nullableBool, Uri? nullableRefType,
                    bool optionalBool = false, Uri optionalRefType = null)
            {}

            public class Model : IArgumentModel
            {
                [Operand] public bool RequiredBool { get; set; }
                [Operand] public bool DefaultBool { get; set; } = true;
                [Operand] public Uri RequiredRefType { get; set; }
                [Operand] public Uri DefaultRefType { get; set; } = new ("http://apple.com");
            }
            // end-snippet
        }

        public class Program_SkipValidation
        {
            public static void Main(string[] args)
            {
                // begin-snippet: arguments_arity_skip_validation
                AppSettings s = new() { Arguments = { SkipArityValidation = true } };
                // end-snippet
            }
        }

        public static BashSnippet Help = new("arguments_arity_help",
            new AppRunner<Program>(), "app.exe", "--help", 0,
            @"Usage: {0} <RequiredBool> <DefaultBool> <RequiredRefType> <DefaultRefType> <requiredBool> <requiredRefType> [<nullableBool> <nullableRefType> <optionalBool> <optionalRefType>]

Arguments:

  RequiredBool     <BOOLEAN>
  Allowed values: true, false

  DefaultBool      <BOOLEAN>  [True]
  Allowed values: true, false

  RequiredRefType  <URI>

  DefaultRefType   <URI>      [http://apple.com/]

  requiredBool     <BOOLEAN>
  Allowed values: true, false

  requiredRefType  <URI>

  nullableBool     <BOOLEAN>
  Allowed values: true, false

  nullableRefType  <URI>

  optionalBool     <BOOLEAN>  [False]
  Allowed values: true, false

  optionalRefType  <URI>");

        public static BashSnippet MissingArgs = new("arguments_arity_missing_args",
            new AppRunner<Program>(), 
            "app.exe", "", 2,
            @"RequiredBool is required
RequiredRefType is required
requiredBool is required
requiredRefType is required");

        public static BashSnippet NoMissingArgs = new("arguments_arity_no_missing_args",
            new AppRunner<Program>(), 
            "app.exe", "true true http://google.com http://google.com true http://google.com", 0,
            "");



        public class Program_Collection
        {
            [DefaultCommand]
            // begin-snippet: arguments_arity_collection
            public void DefaultCommand(
                    [Option('b')] bool[] requiredBool, [Option('u')] Uri[] requiredRefType,
                    [Option] bool[]? nullableBool, [Option] Uri[]? nullableRefType,
                    [Option] bool[] optionalBool = null, [Option] Uri[] optionalRefType = null)
                // end-snippet
            {
            }
        }

        public static BashSnippet Collection_Help = new("arguments_arity_collection_help",
            new AppRunner<Program_Collection>(), "app.exe", "--help", 0,
            @"Usage: {0} [options]

Options:

  -b | --requiredBool (Multiple)     <BOOLEAN>
  Allowed values: true, false

  -u | --requiredRefType (Multiple)  <URI>

  --nullableBool (Multiple)          <BOOLEAN>
  Allowed values: true, false

  --nullableRefType (Multiple)       <URI>

  --optionalBool (Multiple)          <BOOLEAN>
  Allowed values: true, false

  --optionalRefType (Multiple)       <URI>");

        public static BashSnippet Collection_MissingArgs = new("arguments_arity_collection_missing_args",
            new AppRunner<Program_Collection>(),
            "app.exe", "", 2,
            @"requiredBool is required
requiredRefType is required");

        public static BashSnippet Collection_NoMissingArgs = new("arguments_arity_collection_no_missing_args",
            new AppRunner<Program_Collection>(),
            "app.exe", "-b true -b true -u http://google.com -u http://google.com", 0,
            "");

        [Test] public void Obligatory_test_since_snippets_cover_all_cases() => Assert.True(true);
    }
}