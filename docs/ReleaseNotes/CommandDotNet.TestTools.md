# CommandDotNet.TestTools

## 1.1.1

RunInMem will print the stacktrace for of a caught exception before returning the error response, otherwise the stacktrace is lost.

## 1.1.0

Test prompting no longer prints an extraneous `$"IConsole.ReadLine > {input}"` as it's not consistent with console output. This is logged instead.

## 1.0.2

Ensure log provider is always configured for appRunner RunInMem and VerifyScenario extensions

## 1.0.1

make TestToolsLogProvider publicly available

VerifyScenario now captures exceptions that escape AppRunner.Run, putting them into the result to verify as console output.  
This better mimics the behavior of the shell which will return an error code of 1 and print the exception

VerifyScenario will also logging more context on error, including most of AppConfig

improve stack traces with AppOutputBase and PathMap project settings
```
<AppOutputBase>$(MSBuildProjectDirectory)\</AppOutputBase>
<PathMap>$(AppOutputBase)=CommandDotNet/</PathMap>
```