# Interceptors

Interceptors are methods that wrap the execution of a command.

An intercepter must define an InterceptorExecutionDelegate paramenter and can include options, 
[argument models](../Arguments/argument-models.md) and [resolvable parameters](../Extensibility/parameter-resolvers.md)

Defining options in interceptor options makes it easy to define options where they are used.

We'll use an example of a program used to query websites and may need basic auth.

<!-- snippet: getting-started-500-interceptors -->
<a id='snippet-getting-started-500-interceptors'></a>
```c#
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
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/GettingStarted/Getting_Started_500_Interceptors.cs#L10-L78' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting-started-500-interceptors' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

The interceptor method wraps the execution of the targetted command. 
If there are multiple levels of command heirarchy and multiple interceptors in the path to the target command, every interceptor will be executed.

!!! Tip
    You can include CommandContext parameter to get access to tokens, the parse results, AppSettings, invocation pipelines, etc.

    From the `CommandContext.ParseResult`, you can access the target command, separated arguments and unassigned arguments.

    From the `CommandContext.InvocationPipeline`, you can access the instance of the classes containing interceptor methods and the target command method, as well as the MethodInfo and ParameterInfo's.

There are three options defined in the interceptor method: username, password, and verbose.  Let's look at the help to see how they're used.

<!-- snippet: getting-started-500-interceptors-help -->
<a id='snippet-getting-started-500-interceptors-help'></a>
```bash
$ dotnet curl.dll --help
Usage: dotnet curl.dll [command] [options]

Options:

  -u | --username  <TEXT>

  -p | --password  <TEXT>

Commands:

  Get

Use "dotnet curl.dll [command] --help" for more information about a command.
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/getting-started-500-interceptors-help.bash#L1-L16' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting-started-500-interceptors-help' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Notice only two of the options are available at the root of the application: username and password. We can see verbose in the Get command because the option is configured with `AssignToExecutableSubcommands=true`.

<!-- snippet: getting-started-500-interceptors-get-help -->
<a id='snippet-getting-started-500-interceptors-get-help'></a>
```bash
$ dotnet curl.dll Get -h
Usage: dotnet curl.dll Get [options] <uri>

Arguments:

  uri  <URI>

Options:

  -v | --verbose
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/getting-started-500-interceptors-get-help.bash#L1-L12' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting-started-500-interceptors-get-help' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Let's look at how these options are used.

<!-- snippet: getting-started-500-interceptors-get -->
<a id='snippet-getting-started-500-interceptors-get'></a>
```bash
$ dotnet curl.dll -u me -p pwd Get http://mysite.com -v
[GET] http://me:*****@mysite.com/ verbose=True
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/getting-started-500-interceptors-get.bash#L1-L4' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting-started-500-interceptors-get' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Notice password and username must be provided before the Get command and verbose is provided with the Get command.  Using `AssignToExecutableSubcommands` may make the options more intuitive for the user but can increase the noise in the commands and increase opportunity for short name conflicts.
