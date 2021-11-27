## Let's handle Ctrl+C

The above command will take 40 seconds to execute. The way it's currently configured, we have no way to exit early.

With console applications, the standard pattern is to exit the app when Ctrl+C is pressed.  Here's how we support that pattern with CommandDotNet.

<!-- snippet: getting_started_6_ctrlc -->
<a id='snippet-getting_started_6_ctrlc'></a>
```c#
public class Program
{
    static int Main(string[] args) => AppRunner.Run(args);

    public static AppRunner AppRunner => 
        new AppRunner<Program>()
            .UseCancellationHandlers();

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
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/GettingStarted/GettingStarted_6_CtrlC.cs#L11-L37' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting_started_6_ctrlc' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Again, CommandDotNet makes this very easy. Configure the app with `UseCancellationHandlers()` and a `CancellationToken` can be injected into your commands. 

Use either of the two handy extension methods `UntilCancelled` or `ThrowIfCancelled` to exit an enumeration early when cancellation has been requested.
