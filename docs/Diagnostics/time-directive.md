# Timing Commands

## TLDR, How to enable 
Enable the feature with `appRunner.UseTimeDirective()` or `appRunner.UseDefaultMiddleware()`.

## Time Directive

Some shells come with the handy [time command](https://linuxize.com/post/linux-time-command/). 

.Net Core can be executed in environments where the shells do not have the time command, so we've added an alternative

When the `[time]` [directive](../Extensibility/directives.md) is used, a execution time will output to the console after the command has completed.

```bash
$ dotnet example.dll [time] Add 1 2

4

time: 00:00:00.0009052
```
