using System;

namespace CommandDotNet.Tests
{
    public static class AppRunnerBuilderExtensions
    {
        public static AppRunner OnCommandCreated(this AppRunner appRunner, Action<Command> onCommandCreated)
        {
            return appRunner.Configure(cfg => 
                cfg.BuildEvents.OnCommandCreated += args => 
                    onCommandCreated(args.CommandBuilder.Command));
        }
    }
}