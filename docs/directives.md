Directives are special arguments enabling cross cutting features.  We've followed the pattern defined by  [System.CommandLine](https://github.com/dotnet/command-line-api/wiki/Features-overview#debugging) to provide two directives: Debug & Parse

```c#
new AppSettings { EnableDirectives = true }
```

Directives must be the first argument and will be removed from further processing.

### Debug

Sometimes you just need to debug into a process and configuring the debug arguments in VS is too many extra steps.

When you specify `[debug]`, the process id will output to the console and wait for you to attach your debugger.

### Parse

Rules for including spaces in arguments and escaping special characters differ from shell to shell.  Sometimes it's not clear why arguments aren't mapping correctly.

When you specify `[parse]`, the process will exit immediately after printing each argument onto a new line.  This enables you to catch cases where the shell did not parse your arguments as expected.