using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Execution;
using CommandDotNet.Parsing;
using CommandDotNet.TestTools;

namespace CommandDotNet.DocExamples.Extensibility;

public static class Middleware_Examples
{
    public class Program
    {
        public void SomeCommand(string value) 
        {
            Console.WriteLine($"Command executed with: {value}");
        }
    }

    #region Common Patterns

    // begin-snippet: middleware_readonly_pattern
    private static Task<int> LogCommandMiddleware(CommandContext ctx, ExecutionDelegate next)
    {
        // Read from context
        var command = ctx.ParseResult?.TargetCommand?.Name ?? "unknown";
        Console.WriteLine($"Executing: {command}");
        
        // Continue pipeline
        return next(ctx);
    }

    public static AppRunner UseCommandLogging(this AppRunner appRunner)
    {
        return appRunner.Configure(c => c.UseMiddleware(
            LogCommandMiddleware, 
            MiddlewareStages.PostParseInputPreBindValues));
    }
    // end-snippet

    // begin-snippet: middleware_enrichment_pattern
    internal class Database { public Database(string connectionString) { } }
    
    private static Task<int> InjectDatabaseMiddleware(CommandContext ctx, ExecutionDelegate next)
    {
        // Add service for commands to use
        var connectionString = ctx.AppConfig.Services.GetOrThrow<Config>().ConnectionString;
        var db = new Database(connectionString);
        ctx.Services.Add(db);
        
        return next(ctx);
    }

    public static AppRunner UseDatabaseInjection(this AppRunner appRunner, string connectionString)
    {
        return appRunner.Configure(c =>
        {
            c.Services.Add(new Config(connectionString));
            c.UseMiddleware(
                InjectDatabaseMiddleware,
                MiddlewareStages.PostBindValuesPreInvoke);
        });
    }
    // end-snippet

    // begin-snippet: middleware_validation_pattern
    private static Task<int> ValidateArgsMiddleware(CommandContext ctx, ExecutionDelegate next)
    {
        var parseResult = ctx.ParseResult;
        if (parseResult == null)
        {
            return Task.FromResult(ExitCodes.Error);
        }
        
        // Perform validation
        var errors = ValidateArguments(parseResult);
        if (errors.Any())
        {
            foreach (var error in errors)
            {
                ctx.Console.Error.WriteLine(error);
            }
            ctx.ShowHelpOnExit = true;
            return Task.FromResult(ExitCodes.ValidationError);
        }
        
        // Validation passed, continue
        return next(ctx);
    }
    
    private static string[] ValidateArguments(ParseResult parseResult) => Array.Empty<string>();

    public static AppRunner UseCustomValidation(this AppRunner appRunner)
    {
        return appRunner.Configure(c => c.UseMiddleware(
            ValidateArgsMiddleware,
            MiddlewareStages.PostParseInputPreBindValues));
    }
    // end-snippet

    // begin-snippet: middleware_wrapper_pattern
    private static async Task<int> TransactionMiddleware(CommandContext ctx, ExecutionDelegate next)
    {
        var config = ctx.AppConfig.Services.GetOrThrow<Config>();
        var db = ctx.Services.GetOrThrow<Database>();
        
        using var transaction = db.BeginTransaction();
        try
        {
            // Execute command
            var exitCode = await next(ctx);
            
            // Commit or rollback based on result and dryrun setting
            if (exitCode == 0 && !config.DryRun)
            {
                transaction.Commit();
                ctx.Console.WriteLine("Transaction committed");
            }
            else
            {
                transaction.Rollback();
                ctx.Console.WriteLine("Transaction rolled back");
            }
            
            return exitCode;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public static AppRunner UseTransactions(this AppRunner appRunner, string connectionString, bool dryRun = false)
    {
        return appRunner.Configure(c =>
        {
            c.Services.Add(new Config(connectionString) { DryRun = dryRun });
            c.UseMiddleware(
                TransactionMiddleware,
                MiddlewareStages.PostBindValuesPreInvoke);
        });
    }
    // end-snippet

    #endregion

    #region Anti-Patterns

    // begin-snippet: middleware_antipattern_modifying_after_binding
    // BAD - modifying after binding
    private static Task<int> BadMiddleware_ModifyingAfterBinding(CommandContext ctx, ExecutionDelegate next)
    {
        // This happens too late - values are already bound to method parameters
        // This middleware is registered in PostBindValuesPreInvoke but tries to modify argument values
        var arg = ctx.ParseResult?.TargetCommand?.Operands.FirstOrDefault();
        if (arg != null)
        {
            arg.Value = "modified";  // Too late! Already bound to parameters
        }
        return next(ctx);
    }

    // BAD - Wrong stage for modifying argument values
    public static AppRunner UseBadArgumentModifier(this AppRunner appRunner)
    {
        return appRunner.Configure(c => c.UseMiddleware(
            BadMiddleware_ModifyingAfterBinding,
            MiddlewareStages.PostBindValuesPreInvoke));  // Too late! Use PostParseInputPreBindValues instead
    }
    // end-snippet

    // begin-snippet: middleware_antipattern_hiding_exceptions
    // BAD - hiding errors
    private static async Task<int> BadMiddleware_HidingExceptions(CommandContext ctx, ExecutionDelegate next)
    {
        try
        {
            return await next(ctx);
        }
        catch (Exception)
        {
            return 0;  // Pretending everything is fine - BAD!
        }
    }
    // end-snippet

    // begin-snippet: middleware_antipattern_wrong_stage
    // BAD - checking ParseResult in PreTokenize stage
    private static Task<int> BadMiddleware_WrongStage(CommandContext ctx, ExecutionDelegate next)
    {
        var result = ctx.ParseResult;  // null in PreTokenize!
        if (result?.TargetCommand == null)
        {
            return Task.FromResult(ExitCodes.Error);
        }
        return next(ctx);
    }

    // BAD - Registered in wrong stage
    public static AppRunner UseBadParseResultChecker(this AppRunner appRunner)
    {
        return appRunner.Configure(c => c.UseMiddleware(
            BadMiddleware_WrongStage,
            MiddlewareStages.PreTokenize));  // ParseResult not available yet!
    }
    // end-snippet

    // begin-snippet: middleware_antipattern_stateful
    // BAD - shared state between invocations
    public class BadMiddleware_Stateful
    {
        private static int _callCount = 0;  // Shared between runs - BAD!
        
        public static Task<int> Execute(CommandContext ctx, ExecutionDelegate next)
        {
            _callCount++;  // Race conditions in parallel tests!
            Console.WriteLine($"Call count: {_callCount}");
            return next(ctx);
        }
    }
    // end-snippet

    #endregion

    #region Test Support

    internal class Config
    {
        public string ConnectionString { get; }
        public bool DryRun { get; set; }
        
        public Config(string connectionString) => ConnectionString = connectionString;
    }

    internal class Transaction : IDisposable
    {
        public bool WasCommitted { get; private set; }
        public void Commit() => WasCommitted = true;
        public void Rollback() => WasCommitted = false;
        public void Dispose() { }
    }

    #endregion

    #region Testing Examples

    // begin-snippet: middleware_testing_unit
    public class MyApp
    {
        public void Command(string arg) { }
    }

    private static Task<int> MyMiddleware(CommandContext ctx, ExecutionDelegate next)
    {
        if (ctx.ParseResult?.TargetCommand?.Name == "command")
        {
            var arg = ctx.ParseResult.TargetCommand.Operands.FirstOrDefault();
            if (arg?.Value?.ToString() == "--invalid-arg")
            {
                ctx.Console.Error.WriteLine("invalid");
                return Task.FromResult(1);
            }
        }
        return next(ctx);
    }
    // end-snippet

    // begin-snippet: middleware_testing_integration
    private class TestDatabase
    {
        public List<Transaction> Transactions { get; } = new();
        
        public Transaction BeginTransaction()
        {
            var tx = new Transaction();
            Transactions.Add(tx);
            return tx;
        }
    }
    // end-snippet

    // begin-snippet: middleware_testing_capture_state
    public static void CaptureState_Example()
    {
        var result = new AppRunner<MyApp>()
            .Configure(c => c.UseMiddleware(MyMiddleware, MiddlewareStages.PostParseInputPreBindValues))
            .RunInMem("command test");
        
        // Verify middleware execution and state
        // result.ExitCode.Should().Be(0);
        // result.Console.Out.Should().Contain("expected output");
    }
    // end-snippet

    #endregion
}

// Extension methods for test support
internal static class DatabaseExtensions
{
    public static Middleware_Examples.Transaction BeginTransaction(this Middleware_Examples.Database db)
    {
        return new Middleware_Examples.Transaction();
    }
}
