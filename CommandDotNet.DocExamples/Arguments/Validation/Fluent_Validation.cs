using System;
using System.Collections.Generic;
using CommandDotNet.FluentValidation;
using CommandDotNet.NameCasing;
using FluentValidation;
using FluentValidation.Results;
using NUnit.Framework;

namespace CommandDotNet.DocExamples.Arguments.Validation
{
#pragma warning disable CS8618
    [TestFixture]
    public class Fluent_Validation
    {
        // begin-snippet: fluent_validation
        public class Program
        {
            static int Main(string[] args) => AppRunner.Run(args);

            public static AppRunner AppRunner =>
                new AppRunner<Program>()
                    .UseNameCasing(Case.LowerCase)
                    .UseFluentValidation();

            public void Create(IConsole console, Table table, Host host, Verbosity verbosity)
            {
                console.WriteLine($"created {table.Name} as {host.Server}. notifying: {table.Owner}");
            }
        }

        public class Host : IArgumentModel
        {
            [Option]
            public string Server { get; set; }
        }

        public class Table : IArgumentModel
        {
            [Operand]
            public string Name { get; set; }

            [Option]
            public string Owner { get; set; }
        }

        public class Verbosity : IArgumentModel
        {
            [Option('q')]
            public bool Quiet { get; set; }
            [Option('v')]
            public bool Verbose { get; set; }
        }

        public class HostValidator : AbstractValidator<Host>
        {
            public HostValidator()
            {
                RuleFor(h => h.Server)
                    .NotNull().NotEmpty()
                    .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                    .WithMessage("sever is not a valid fully-qualified http, https, or ftp URL");
            }
        }

        public class TableValidator : AbstractValidator<Table>
        {
            public TableValidator()
            {
                RuleFor(t => t.Name).NotNull().NotEmpty().MaximumLength(10);
                RuleFor(t => t.Owner).NotNull().NotEmpty().EmailAddress();
            }
        }

        public class VerbosityValidator : AbstractValidator<Verbosity>
        {
            public VerbosityValidator()
            {
                When(v => v.Verbose,
                    () => RuleFor(v => v.Quiet)
                        .NotEqual(true)
                        .WithMessage("quiet and verbose are mutually exclusive. There can be only one!"));
            }
        }
        // end-snippet

        public class Program_WithFactory
        {
            static int Main(string[] args) => AppRunner.Run(args);

            // begin-snippet: fluent_validation_factory
            public static AppRunner AppRunner =>
                new AppRunner<Program>()
                    .UseNameCasing(Case.LowerCase)
                    .UseFluentValidation(validatorFactory: model =>
                    {
                        switch (model)
                        {
                            case Host: return new HostValidator();
                            case Table: return new TableValidator();
                            case Verbosity: return new VerbosityValidator();
                            default: return null;
                        }
                    });
            // end-snippet
        }

        public static BashSnippet Create_Invalid = new("fluent_validation_create_invalid",
            Program.AppRunner,
            "dotnet table.dll", "create TooLongTableName --server bossman --owner abc -qv", 2,
            @"'Table' is invalid
  The length of 'Name' must be 10 characters or fewer. You entered 16 characters.
  'Owner' is not a valid email address.
'Host' is invalid
  sever is not a valid fully-qualified http, https, or ftp URL
'Verbosity' is invalid
  quiet and verbose are mutually exclusive. There can be only one!");

        public static BashSnippet Factory_Create_Invalid = new("fluent_validation_factory_create_invalid",
            Program_WithFactory.AppRunner,
            "dotnet table.dll", "create TooLongTableName --server bossman --owner abc -qv", 2,
            @"'Table' is invalid
  The length of 'Name' must be 10 characters or fewer. You entered 16 characters.
  'Owner' is not a valid email address.
'Host' is invalid
  sever is not a valid fully-qualified http, https, or ftp URL
'Verbosity' is invalid
  quiet and verbose are mutually exclusive. There can be only one!");

        [Test] public void Obligatory_test_since_snippets_cover_all_cases() => Assert.True(true);
    }
}