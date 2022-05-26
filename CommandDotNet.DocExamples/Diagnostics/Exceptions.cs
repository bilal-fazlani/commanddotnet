using System;
using NUnit.Framework;
using CommandDotNet.Diagnostics;
using CommandDotNet.TestTools;

namespace CommandDotNet.DocExamples.Diagnostics
{
    public class Exceptions
    {
        public class Program_Simple
        {
            // begin-snippet: exceptions_simple
            public class Program
            {
                static int Main(string[] args) => AppRunner.Run(args);

                public static AppRunner AppRunner =>
                    new AppRunner<Program>()
                        .UseErrorHandler(ErrorHandler);

                private static int ErrorHandler(CommandContext? ctx, Exception exception)
                {
                    var errorWriter = (ctx?.Console.Error ?? Console.Error);

                    exception.Print(errorWriter.WriteLine,
                        includeProperties: true,
                        includeData: true,
                        includeStackTrace: false);

                    errorWriter.WriteLine();

                    // print help for the target command or root command
                    // if the exception occurred before a command could be parsed
                    ctx?.PrintHelp();

                    return ExitCodes.Error.Result;
                }

                public void Throw(string message)
                {
                    throw new ArgumentException(message, nameof(message))
                    {
                        Data = { { "method", nameof(Exceptions.Throw) } }
                    };
                }
            }
            // end-snippet
        }

        public class Program_UseErrorHandler_Delegate
        {
            public class Program
            {
                // begin-snippet: exceptions_use_error_handler_delegate
                static int Main(string[] args)
                {
                    return new AppRunner<Program>()
                        .Configure(b =>
                        {
                            // some other setup that could throw exceptions
                            // i.e. configure containers, load configs, register custom middleware
                        })
                        .UseErrorHandler((ctx, ex) =>
                        {
                            (ctx?.Console.Error ?? Console.Error).WriteLine(ex.Message);
                            return ExitCodes.Error.Result;
                        })
                        .Run(args);
                }
                // end-snippet

                public void Throw(string message)
                {
                    throw new ArgumentException(message, nameof(message))
                    {
                        Data = { { "method", nameof(Exceptions.Throw) } }
                    };
                }
            }
        }

        public class Program_TryCatch
        {
            public class Program
            {
                // begin-snippet: exceptions_try_catch
                static int Main(string[] args)
                {
                    try
                    {
                        // some other setup that could throw exceptions
                        // i.e. configure containers, load configs, register custom middleware

                        return new AppRunner<Program>().Run(args);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine(ex.Message);
                        return ExitCodes.Error.Result;
                    }
                }
                // end-snippet

                public void Throw(string message)
                {
                    throw new ArgumentException(message, nameof(message))
                    {
                        Data = { { "method", nameof(Exceptions.Throw) } }
                    };
                }
            }
        }

        public class Program_CommandLogger
        {
            public class Program
            {
                // begin-snippet: exceptions_cmdlog_error_handler
                static int Main(string[] args) => AppRunner.Run(args);

                public static AppRunner AppRunner =>
                    new AppRunner<Program>()
                        .UseErrorHandler(ErrorHandler);

                private static int ErrorHandler(CommandContext? ctx, Exception exception)
                {
                    var errorWriter = (ctx?.Console.Error ?? Console.Error);
                    exception.Print(errorWriter.WriteLine,
                        includeProperties: true,
                        includeData: true,
                        includeStackTrace: false);

                    // use CommandLogger if it has not already logged for this CommandContext
                    if (ctx is not null && !CommandLogger.HasLoggedFor(ctx))
                    {
                        CommandLogger.Log(ctx,
                            writer: errorWriter.WriteLine,
                            includeSystemInfo: true,
                            includeMachineAndUser: true,
                            includeAppConfig: false
                        );
                    }

                    // print help for the target command or root command
                    // if the exception occurred before a command could be parsed
                    ctx?.PrintHelp();

                    return ExitCodes.Error.Result;
                }

                public void Throw(string message)
                {
                    throw new ArgumentException(message, nameof(message))
                    {
                        Data = { { "method", nameof(Exceptions.Throw) } }
                    };
                }
                // end-snippet
            }
        }

        public static BashSnippet Throw = new ("exceptions_throw", 
            Program_Simple.Program.AppRunner, 
            "example.exe", "Throw yikes", 1, 
            @"System.ArgumentException: yikes (Parameter 'message')
Properties:
  Message: yikes (Parameter 'message')
  ParamName: message
Data:
  method: Throw

Usage: example.exe Throw <message>

Arguments:

  message  <TEXT>");

        private static TestEnvironment TestEnvironment = new()
        {
            FrameworkDescription = ".NET 5.0.13",
            OSDescription = "Microsoft Windows 10.0.12345",
            MachineName = "my-machine",
            UserName = "my-machine\\username"
        };

        public static BashSnippet Throw_CmdLog = new("exceptions_throw_cmdlog",
            Program_CommandLogger.Program.AppRunner.UseTestEnv(TestEnvironment), 
            "example.exe", "Throw yikes", 1,
            @"System.ArgumentException: yikes (Parameter 'message')
Properties:
  Message: yikes (Parameter 'message')
  ParamName: message
Data:
  method: Throw

***************************************
Original input:
  Throw yikes

command: Throw

arguments:

  message <Text>
    value: yikes
    inputs: yikes
    default:

Tool version  = doc-examples.dll 1.1.1.1
.Net version  = .NET 5.0.13
OS version    = Microsoft Windows 10.0.12345
Machine       = my-machine
Username      = \my-machine\username
***************************************

Usage: example.exe Throw <message>

Arguments:

  message  <TEXT>");


        [Test]
        public void UseErrorHandler_Delegate_works()
        {
            var (exitCode, output) = typeof(Program_UseErrorHandler_Delegate.Program).InvokeMainMethod("Throw yikes");
            Assert.AreEqual(1, exitCode);
            Assert.AreEqual($"yikes (Parameter 'message'){Environment.NewLine}", output);
        }

        [Test] 
        public void TryCatch_works()
        {
            var (exitCode, output) = typeof(Program_TryCatch.Program).InvokeMainMethod("Throw yikes");
            Assert.AreEqual(1, exitCode);
            Assert.AreEqual($"yikes (Parameter 'message'){Environment.NewLine}", output);
        }
    }
}