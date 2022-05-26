using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CommandDotNet.DataAnnotations;
using CommandDotNet.NameCasing;
using NUnit.Framework;

namespace CommandDotNet.DocExamples.DataAnnotations
{
#pragma warning disable CS8618
    [TestFixture]
    public class DataAnnotations_1_Table
    {
        // begin-snippet: dataannotations-1-table
        public class Program
        {
            static int Main(string[] args) => AppRunner.Run(args);

            public static AppRunner AppRunner => 
                new AppRunner<Program>()
                    .UseNameCasing(Case.LowerCase)
                    .UseDataAnnotationValidations();
            
            public Task<int> Interceptor(InterceptorExecutionDelegate next, Verbosity verbosity)
            {
                // pre-execution logic here

                return next(); // Create method is executed here

                // post-execution logic here
            }

            public void Create(IConsole console, Table table, [Option, Url] string server)
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
            [Option('s', AssignToExecutableSubcommands = true)]
            public bool Silent { get; set; }
            [Option('v', AssignToExecutableSubcommands = true)]
            public bool Verbose { get; set; }

            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                if (Silent && Verbose)
                    yield return new ValidationResult("silent and verbose are mutually exclusive. There can be only one!");
            }
        }
        // end-snippet

        public static BashSnippet Help = new("DataAnnotations-1-table-create-help",
            Program.AppRunner,
            "dotnet table.dll", "create --help", 0,
            @"Usage: {0} create [options] <name>

Arguments:

  name  <TEXT>

Options:

  --owner         <TEXT>

  --server        <TEXT>

  -s | --silent

  -v | --verbose");

        public static BashSnippet Hire = new("DataAnnotations-1-table-create",
            Program.AppRunner,
            "dotnet hr.dll", "create TooLongTableName --server bossman --owner abc -sv", 2,
            @"silent and verbose are mutually exclusive. There can be only one!
'server' is not a valid fully-qualified http, https, or ftp URL.
'name' must be a string or array type with a maximum length of '10'.
'owner' is not a valid e-mail address.");

        [Test] public void Obligatory_test_since_snippets_cover_all_cases() => Assert.True(true);
    }
}