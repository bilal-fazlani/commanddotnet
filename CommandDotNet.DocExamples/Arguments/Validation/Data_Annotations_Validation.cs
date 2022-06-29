using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CommandDotNet.DataAnnotations;
using CommandDotNet.NameCasing;
using NUnit.Framework;

namespace CommandDotNet.DocExamples.Arguments.Validation
{
#pragma warning disable CS8618
    [TestFixture]
    public class Data_Annotations_Validation
    {
        // begin-snippet: data_annotations_validation
        public class Program
        {
            static int Main(string[] args) => AppRunner.Run(args);

            public static AppRunner AppRunner =>
                new AppRunner<Program>()
                    .UseNameCasing(Case.LowerCase)
                    .UseDataAnnotationValidations();

            public void Create(IConsole console, Table table, [Option, Url] string server, Verbosity verbosity)
            {
                console.WriteLine($"created {table.Name} as {server}. notifying: {table.Owner}");
            }
        }

        public class Table : IArgumentModel
        {
            [Operand, Required, MaxLength(10)]
            public string Name { get; set; }

            [Option, Required, EmailAddress]
            public string Owner { get; set; }
        }

        public class Verbosity : IArgumentModel, IValidatableObject
        {
            [Option('q')]
            public bool Quiet { get; set; }
            [Option('v')]
            public bool Verbose { get; set; }

            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                if (Quiet && Verbose)
                    yield return new ValidationResult("quiet and verbose are mutually exclusive. There can be only one!");
            }
        }
        // end-snippet

        public static BashSnippet Help = new("data_annotations_validation_create_help",
            Program.AppRunner,
            "dotnet table.dll", "create --help", 0,
            @"Usage: {0} create [options] <name>

Arguments:

  name  <TEXT>

Options:

  --owner         <TEXT>

  --server        <TEXT>

  -q | --quiet

  -v | --verbose");

        public static BashSnippet Create_Invalid = new("data_annotations_validation_create_invalid",
            Program.AppRunner,
            "dotnet table.dll", "create TooLongTableName --server bossman --owner abc -qv", 2,
            @"'server' is not a valid fully-qualified http, https, or ftp URL.
'name' must be a string or array type with a maximum length of '10'.
'owner' is not a valid e-mail address.
quiet and verbose are mutually exclusive. There can be only one!");

        [Test] public void Obligatory_test_since_snippets_cover_all_cases() => Assert.True(true);
    }
}