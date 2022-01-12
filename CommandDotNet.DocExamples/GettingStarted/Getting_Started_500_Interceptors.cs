using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace CommandDotNet.DocExamples.GettingStarted
{
    [TestFixture]
    public class Getting_Started_500_Interceptors
    {
        // begin-snippet: getting-started-500-interceptors
        public class Program
        {
            static int Main(string[] args) => AppRunner.Run(args);
            public static AppRunner AppRunner => new AppRunner<Curl>();
        }

        public class Curl
        {
            private readonly IConsole _console;
            private string? _username;
            private Password? _password;
            private bool? _verbose;

            // Constructors can take any resolvable parameters. IConsole, CommandContext, 
            // and CancellationToken when UseCancellationHandlers is configured.
            public Curl(IConsole console)
            {
                _console = console;
            }

            // Method name does not matter but must include an InterceptorExecutionDelegate parameter.
            // All other parameters listed here are optional.
            public Task<int> Interceptor(
                InterceptorExecutionDelegate next, 
                CommandContext ctx,
                // options are assigned to this command
                // and must be provided before any child commands
                [Option('u')] string? username,
                [Option('p')] Password? password,
                // AssignToExecutableSubcommands moves the option to the final commands
                // and cannot be provided with this command
                [Option('v', AssignToExecutableSubcommands = true)] bool? verbose
            )
            {
                _username = username;
                _password = password;
                _verbose = verbose;

                // access to AppConfig, AppSettings and other services
                var settings = ctx.AppConfig.AppSettings;
                // access to parse results, including remaining and separated arguments 
                var parseResult = ctx.ParseResult;
                // access to target command method and its hosting object,
                // and all interceptor methods in the path to the target command.
                var pipeline = ctx.InvocationPipeline;

                // pre-execution logic here

                // next() will execute the TargetCommand and all
                // remaining interceptors in the ctx.InvocationPipeline
                var result = next();

                // post-execution logic here

                return result;
            }

            public void Get(Uri uri) => _console.WriteLine($"[GET] {BuildBasicAuthUri(uri)} verbose={_verbose}");
            
            private string BuildBasicAuthUri(Uri uri)
            {
                var uriString = uri.ToString();
                return _username is null
                    ? uriString
                    : $"{uri.Scheme}://{_username}:{_password}@" + uriString.Substring(uri.Scheme.Length + 3);
            }
        }
        // end-snippet

        public static BashSnippet Help = new("getting-started-500-interceptors-help",
            Program.AppRunner,
            "dotnet curl.dll", "--help", 0,
            @"Usage: dotnet curl.dll [command] [options]

Options:

  -u | --username  <TEXT>

  -p | --password  <TEXT>

Commands:

  Get

Use ""dotnet curl.dll [command] --help"" for more information about a command.");

        public static BashSnippet Help_Add = new("getting-started-500-interceptors-get-help",
            Program.AppRunner,
            "dotnet curl.dll", "Get -h", 0,
            @"Usage: dotnet curl.dll Get [options] <uri>

Arguments:

  uri  <URI>

Options:

  -v | --verbose");

        public static BashSnippet Get = new("getting-started-500-interceptors-get",
            Program.AppRunner,
            "dotnet curl.dll", "-u me -p pwd Get http://mysite.com -v", 0,
            @"[GET] http://me:*****@mysite.com/ verbose=True");

        [Test] public void Obligatory_test_since_snippets_cover_all_cases() => Assert.True(true);
    }
}