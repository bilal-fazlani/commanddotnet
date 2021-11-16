using System;
using System.Collections;
using static System.Environment;

namespace CommandDotNet
{
    public interface IEnvironment
    {
        string CommandLine { get; }
        string CurrentDirectory { get; set; }
        int CurrentManagedThreadId { get; }
        int ExitCode { get; set; }
        string FrameworkDescription { get; }
        bool HasShutdownStarted { get; }
        bool Is64BitOperatingSystem { get; }
        bool Is64BitProcess { get; }
        string MachineName { get; }
        string OSDescription { get; }
        OperatingSystem OSVersion { get; }
        int ProcessorCount { get; }
        int ProcessId { get; }
        string SystemDirectory { get; }
        int TickCount { get; }
        long TickCount64 { get; }
        bool UserInteractive { get; }
        string UserDomainName { get; }
        string UserName { get; }
        Version Version { get; }

        void Exit(int exitCode);
        void FailFast(string? message, Exception? exception);
        
        string[] GetCommandLineArgs();

        string GetFolderPath(SpecialFolder folder, SpecialFolderOption option = SpecialFolderOption.None);

        string ExpandEnvironmentVariables(string name);
        string? GetEnvironmentVariable(string variable, EnvironmentVariableTarget? target = null);
        IDictionary GetEnvironmentVariables();
        void SetEnvironmentVariables(string variable, string? value, EnvironmentVariableTarget? target);
    }
}