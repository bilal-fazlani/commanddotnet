using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace CommandDotNet.DocExamples.Arguments.Arguments
{
#pragma warning disable CS8618
    
    [TestFixture]
    public class Argument_Models
    {
        public class Program_WithoutModel
        {
            // begin-snippet: argument_models_notify_without_model
            public void Notify(string message, List<string> recipients,
                [Option] bool dryrun, [Option('v')] bool verbose, [Option('q')] bool quiet)
            {
                // send notification
            }
            // end-snippet
        }

        public class Program_WithModel
        {
            // begin-snippet: argument_models_notify_with_model
            public void Notify(
                NotificationArgs notificationArgs,
                DryRunOptions dryRunOptions, 
                VerbosityOptions verbosityOptions)
            {
                // send notification
            }

            public class NotificationArgs : IArgumentModel
            {
                [Operand]
                public string Message { get; set; } = null!;

                [Operand]
                public List<string> Recipients { get; set; } = null!;
            }
            // end-snippet
        }

        public class Program_WithComposedModel
        {
            // begin-snippet: argument_models_notify_with_model_composed
            public void Notify(NotificationArgs notificationArgs)
            {
                // send notification
            }

            public class NotificationArgs : IArgumentModel
            {
                [Operand]
                public string Message { get; set; } = null!;

                [Operand]
                public List<string> Recipients { get; set; } = null!;

                public DryRunOptions DryRunOptions { get; set; } = null!;

                public VerbosityOptions VerbosityOptions { get; set; } = null!;
            }
            // end-snippet
        }

        public class Program_WithInterceptor
        {
            // begin-snippet: argument_models_notify_with_interceptor

            public Task<int> Interceptor(InterceptorExecutionDelegate next, CommandContext ctx,
                DryRunOptions dryRunOptions, VerbosityOptions verbosityOptions)
            {
                IEnumerable<IArgumentModel> models = ctx.InvocationPipeline.All
                    .SelectMany(s => s.Invocation.FlattenedArgumentModels);
                return next();
            }

            public void Notify(NotificationArgs notificationArgs)
            {
                // send notification
            }

            public class NotificationArgs : IArgumentModel
            {
                [Operand]
                public string Message { get; set; } = null!;

                [Operand]
                public List<string> Recipients { get; set; } = null!;
            }
            
            public class DryRunOptions : IArgumentModel
            {
                [Option("dryrun", AssignToExecutableSubcommands = true)]
                public bool IsDryRun { get; set; } = false;
            }

            public class VerbosityOptions : IArgumentModel
            {
                [Option('v', AssignToExecutableSubcommands = true)]
                public bool Verbose { get; set; }

                [Option('q', AssignToExecutableSubcommands = true)]
                public bool Quite { get; set; }
            }

            // end-snippet
        }

        public class Program_WithNestedOperands
        {
            public void Notify(NotifyModel notifyModel)
            {
                // send notification
            }

            // begin-snippet: argument_models_notify_with_nested_operands_model
            public class NotifyModel : IArgumentModel
            {
                [OrderByPositionInClass]
                public NotificationArgs NotificationArgs { get; set; }
                public DryRunOptions DryRunOptions { get; set; }
                public VerbosityOptions VerbosityOptions { get; set; }
            }
            // end-snippet

            public class NotificationArgs : IArgumentModel
            {
                [Operand]
                public string Message { get; set; } = null!;

                [Operand]
                public List<string> Recipients { get; set; } = null!;
            }
        }

        public class Program_WithInvalidhNestedOperands
        {
            public void Notify(NotifyModel notifyModel)
            {
                // send notification
            }

            // begin-snippet: argument_models_notify_with_invalid_nested_operands_model
            public class NotifyModel : IArgumentModel
            {
                public NotificationArgs NotificationArgs { get; set; }
                public DryRunOptions DryRunOptions { get; set; }
                public VerbosityOptions VerbosityOptions { get; set; }
            }
            // end-snippet

            public class NotificationArgs : IArgumentModel
            {
                [Operand]
                public string Message { get; set; } = null!;

                [Operand]
                public List<string> Recipients { get; set; } = null!;
            }
        }

        // begin-snippet: argument_models_dry_run_and_verbosity
        public class DryRunOptions : IArgumentModel
        {
            [Option("dryrun")]
            public bool IsDryRun { get; set; } = false;
        }
        
        public class VerbosityOptions : IArgumentModel, IValidatableObject
        {
            [Option('v')]
            public bool Verbose { get; set; }

            [Option('q')]
            public bool Quite { get; set; }

            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                // use the CommandDotNet.DataAnnotations package to run this validation
                if (Verbose && Quite)
                    yield return new ValidationResult("Verbose and Quiet are mutually exclusive. Choose one or the other.");
            }
        }
        // end-snippet

        public static BashSnippet Help_WithoutModel = new("argument_models_notify_without_model_help",
            new AppRunner<Program_WithoutModel>(), "myapp.exe", "Notify --help", 0,
            @"Usage: {0} Notify [options] <message> <recipients>

Arguments:

  message                <TEXT>

  recipients (Multiple)  <TEXT>

Options:

  --dryrun

  -v | --verbose

  -q | --quiet");

        public static BashSnippet Help_WithModel = new("argument_models_notify_with_model_help",
            new AppRunner<Program_WithModel>(), "myapp.exe", "Notify --help", 0,
            @"Usage: {0} Notify [options] <Message> <Recipients>

Arguments:

  Message                <TEXT>

  Recipients (Multiple)  <TEXT>

Options:

  --dryrun

  -v | --Verbose

  -q | --Quite");

        public static BashSnippet Help_WithModelComposed = new("argument_models_notify_with_model_composed_help",
            new AppRunner<Program_WithComposedModel>(), "myapp.exe", "Notify --help", 0,
            Help_WithModel.Output);

        public static BashSnippet Help_WithInterceptor = new("argument_models_notify_with_interceptor_help",
            new AppRunner<Program_WithInterceptor>(), "myapp.exe", "Notify --help", 0,
            @"Usage: {0} Notify [options] <Message> <Recipients>

Arguments:

  Message                <TEXT>

  Recipients (Multiple)  <TEXT>

Options:

  --dryrun

  -v | --Verbose

  -q | --Quite");

        public static BashSnippet Help_WithNestedOperands = new("argument_models_notify_with_nested_operands_help",
            new AppRunner<Program_WithNestedOperands>(), "myapp.exe", "Notify --help", 0,
            @"Usage: {0} Notify [options] <Message> <Recipients>

Arguments:

  Message                <TEXT>

  Recipients (Multiple)  <TEXT>

Options:

  --dryrun

  -v | --Verbose

  -q | --Quite");

        public static BashSnippet Help_WithInvalidNestedOperands = new("argument_models_notify_with_invalid_nested_operands_help",
            new AppRunner<Program_WithInvalidhNestedOperands>(), "myapp.exe", "Notify --help", 1,
            @"CommandDotNet.InvalidConfigurationException: Operand property must be attributed with OperandAttribute or OrderByPositionInClassAttribute to guarantee consistent order. Properties:
  CommandDotNet.DocExamples.Arguments.Arguments.Argument_Models+Program_WithInvalidhNestedOperands+NotifyModel.NotificationArgs");

        [Test] public void Obligatory_test_since_snippets_cover_all_cases() => Assert.True(true);
    }
}