# Ctrl+C

The last example from the [Piped arguments](getting-started-700-pipes.md) page

```bash
~
$ dotnet linq.dll Range 1 4 10000 | dotnet linq.dll Sum
1
3
6
10
```

will take 40 seconds to execute. The way it's currently configured, the user is stuck waiting until the command completes.

With console applications, the standard pattern is to exit the app when Ctrl+C is pressed.  Here's how we support that pattern with CommandDotNet.

<!-- snippet: getting-started-800-ctrlc -->
<a id='snippet-getting-started-800-ctrlc'></a>
```c#
public class Program
{
    static int Main(string[] args) => AppRunner.Run(args);

    public static AppRunner AppRunner => 
        new AppRunner<Program>()
            .UseCancellationHandlers();

    // could also use .UseDefaultMiddleware()

    public void Range(IConsole console, CancellationToken ct, int start, int count, int sleep = 0)
    {
        foreach (var i in Enumerable.Range(start, count).UntilCancelled(ct, sleep))
        {
            console.WriteLine(i);
        }
    }

    public void Sum(IConsole console, CancellationToken ct, IEnumerable<int> values)
    {
        int total = 0;
        foreach (var value in values.ThrowIfCancelled(ct))
        {
            console.WriteLine(total += value);
        }
    }
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/GettingStarted/Getting_Started_800_CtrlC.cs#L11-L39' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting-started-800-ctrlc' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Again, CommandDotNet makes this very easy. Configure the app with `appRunner.UseCancellationHandlers()` which configures a `CancellationToken` that can be injected into your command and interceptor methods. 

Use either of the two handy extension methods `UntilCancelled` or `ThrowIfCancelled` to exit an enumeration early when cancellation has been requested.
