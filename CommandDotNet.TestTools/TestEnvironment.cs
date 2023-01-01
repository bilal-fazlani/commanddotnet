using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Extensions;
using CommandDotNet.Tokens;

namespace CommandDotNet.TestTools
{
#pragma warning disable CS8618
    
    public class TestEnvironment : IEnvironment
    {
        public string[]? CommandLineArgs;
        public Dictionary<EnvironmentVariableTarget, Dictionary<string, string?>> EnvVarByTarget = new();
        public Action<int>? OnExit;
        public Action<(string? message, Exception? exception)>? OnFailFast;
        public Func<string, string>? OnExpandEnvironmentVariables;

        public string CommandLine { get; set; }
        public string CurrentDirectory { get; set; }
        public int CurrentManagedThreadId { get; set; }
        public int ExitCode { get; set; }
        public string FrameworkDescription { get; set; }
        public bool HasShutdownStarted { get; set; }
        public bool Is64BitOperatingSystem { get; set; }
        public bool Is64BitProcess { get; set; }
        public string MachineName { get; set; }
        public string OSDescription { get; set; }
        public OperatingSystem OSVersion { get; set; }
        public int ProcessorCount { get; set; }
        public int ProcessId { get; set; }
        public string SystemDirectory { get; set; }
        public int TickCount { get; set; }
        public long TickCount64 { get; set; }
        public bool UserInteractive { get; set; }
        public string UserDomainName { get; set; }
        public string UserName { get; set; }
        public Version Version { get; set; }

        public void Exit(int exitCode)
        {
            ExitCode = exitCode;
            OnExit?.Invoke(exitCode);
        }

        public void FailFast(string? message, Exception? exception) => OnFailFast?.Invoke((message, exception));

        public string[] GetCommandLineArgs() => CommandLineArgs 
            ??= CommandLineStringSplitter.Instance.Split(CommandLine).ToArray();

        public string GetFolderPath(Environment.SpecialFolder folder, Environment.SpecialFolderOption option = Environment.SpecialFolderOption.None) => 
            Environment.GetFolderPath(folder, option);

        public string ExpandEnvironmentVariables(string name) =>
            OnExpandEnvironmentVariables is null
                ? Environment.ExpandEnvironmentVariables(name)
                : OnExpandEnvironmentVariables(name);

        public string? GetEnvironmentVariable(string variable, EnvironmentVariableTarget? target = null) =>
            target is not null
                ? EnvVarByTarget.GetValueOrDefault(target.Value)?.GetValueOrDefault(variable)
                : (EnvVarByTarget.GetValueOrDefault(EnvironmentVariableTarget.Process)
                   ?? EnvVarByTarget.GetValueOrDefault(EnvironmentVariableTarget.User)
                   ?? EnvVarByTarget.GetValueOrDefault(EnvironmentVariableTarget.Machine))
                ?.GetValueOrDefault(variable);

        public IDictionary GetEnvironmentVariables() => EnvVarByTarget
            .SelectMany(d => d.Value)
            .ToDictionary(d => d.Key, d => d.Value);

        public void SetEnvironmentVariables(string variable, string? value, EnvironmentVariableTarget? target)
        {
            target ??= EnvironmentVariableTarget.Process;
            var vars = EnvVarByTarget.GetOrAdd(target.Value, 
                key => new Dictionary<string, string?>());
            vars[variable] = value;
        }
    }
}