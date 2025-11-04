# Debugging

## TLDR, How to enable 
Enable the feature with `appRunner.UseDebugDirective()` or `appRunner.UseDefaultMiddleware()`.

## Debug Directive

Sometimes you just need to debug into a process and configuring the debug arguments in VS is too many extra steps.

When the `[debug]` [directive](../Extensibility/directives.md) is used, the process id will output to the console and wait for you to attach a debugger.

```bash
$ dotnet example.dll [debug] Add 1 2

Attach your debugger to process 24236 ({exe name}).
```

## Debugger from the start
The debugger is attached early in the middleware pipeline, after the args have been tokenized. 
Any logic before appRunner.Run will occur before stepping into the debugger.
To attach immediately in the Main method, use `Debugger.AttachIfDebugDirective(args)`.

<!-- snippet: diagnostics_debug_directive -->
<a id='snippet-diagnostics_debug_directive'></a>
```cs
class Program
{
    static int Main(string[] args)
    {
        Debugger.AttachIfDebugDirective(args);
        return new AppRunner<App>().Run(args);
    }
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Diagnostics/Diagnostics_Examples.cs#L12-L21' title='Snippet source file'>snippet source</a> | <a href='#snippet-diagnostics_debug_directive' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->
