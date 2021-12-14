# Piped arguments

<!-- snippet: getting-started-700-pipes -->
<a id='snippet-getting-started-700-pipes'></a>
```c#
public class Program
{
    static int Main(string[] args) => AppRunner.Run(args);

    public static AppRunner AppRunner => new AppRunner<Program>();

    public void Range(IConsole console, int start, int count, int sleep = 0)
    {
        foreach (var i in Enumerable.Range(start, count))
        {
            console.WriteLine(i);
            if (sleep > 0)
            {
                Thread.Sleep(sleep);
            }
        }
    }

    public void Sum(IConsole console, IEnumerable<int> values)
    {
        int total = 0;
        foreach (var value in values)
        {
            console.WriteLine(total += value);
        }
    }
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/GettingStarted/Getting_Started_700_Pipes.cs#L11-L39' title='Snippet source file'>snippet source</a> | <a href='#snippet-getting-started-700-pipes' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Here we've converted the arguments for Sum into an IEnumerable<int> and added a Range command.
You've probably noticed these commands wrap LINQ methods of the same name. 
We've added an optional sleep option to Range to better mimic a long running stream. 

We could have used List<int>, int[], or any other collection type. 
Using IEnumerable<T> allows the command to start processing before the stream has completed.

Very few console frameworks make it this easy to write streaming console tools.

Let's see it in action:

```bash
~
$ dotnet linq.dll Range 1 4 10000 | dotnet linq.dll Sum
1
3
6
10
```

After outputtting a value, Range sleeps for 10 seconds.  We know Sum is streaming because it immediatly outputs the new sum as soon as it receives a value and waits for the next value.
