using System;
using System.Collections;

namespace CommandDotNet.Extensions
{
    public class SystemEnvironment : IEnvironment
    {
        public virtual string CommandLine => Environment.CommandLine;
        public virtual string CurrentDirectory
        {
            get => Environment.CurrentDirectory;
            set => Environment.CurrentDirectory = value;
        }
        public virtual int CurrentManagedThreadId => Environment.CurrentManagedThreadId;
        public virtual int ExitCode
        {
            get => Environment.ExitCode;
            set => Environment.ExitCode = value;
        }
        public virtual string FrameworkDescription => System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription.Trim();
        public virtual bool HasShutdownStarted => Environment.HasShutdownStarted;
        public virtual bool Is64BitOperatingSystem => Environment.Is64BitOperatingSystem;
        public virtual bool Is64BitProcess => Environment.Is64BitProcess;
        public virtual string MachineName => Environment.MachineName;
        public virtual string OSDescription => System.Runtime.InteropServices.RuntimeInformation.OSDescription.Trim();
        public virtual OperatingSystem OSVersion => Environment.OSVersion;
        public virtual int ProcessorCount => Environment.ProcessorCount;
        public virtual int ProcessId => Environment.ProcessId;
        public virtual string SystemDirectory => Environment.SystemDirectory;
        public virtual int TickCount => Environment.TickCount;
        public virtual long TickCount64 => Environment.TickCount64;
        public virtual bool UserInteractive => Environment.UserInteractive;
        public virtual string UserDomainName => Environment.UserDomainName;
        public virtual string UserName => Environment.UserName;
        public virtual Version Version => Environment.Version;

        public virtual void Exit(int exitCode) => Environment.Exit(exitCode);
        public virtual void FailFast(string? message, Exception? exception) => Environment.FailFast(message, exception);

        public virtual string[] GetCommandLineArgs() => Environment.GetCommandLineArgs();
        public virtual string GetFolderPath(Environment.SpecialFolder folder, 
            Environment.SpecialFolderOption option = Environment.SpecialFolderOption.None) => Environment.GetFolderPath(folder, option);

        public virtual string ExpandEnvironmentVariables(string name) => Environment.ExpandEnvironmentVariables(name);
        public virtual string? GetEnvironmentVariable(string variable, EnvironmentVariableTarget? target = null) => 
            target is null
                ? Environment.GetEnvironmentVariable(variable)
                : Environment.GetEnvironmentVariable(variable, target.Value);
        public virtual IDictionary GetEnvironmentVariables() => Environment.GetEnvironmentVariables();

        public virtual void SetEnvironmentVariables(string variable, string? value, EnvironmentVariableTarget? target)
        {
            if (target is null)
            {
                Environment.SetEnvironmentVariable(variable, value);
            }
            else
            {
                Environment.SetEnvironmentVariable(variable, value, target.Value);
            }
        }
    }
}