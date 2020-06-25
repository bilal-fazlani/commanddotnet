using System;
using System.Threading.Tasks;
using CommandDotNet.Execution;

namespace CommandDotNet.ReadLineRepl
{
    public static class ReplMiddleware
    {
        public static AppRunner UseRepl(this AppRunner appRunner, ReplConfig? replConfig = null)
        {
            ReadLine.HistoryEnabled = true;

            replConfig ??= new ReplConfig();
            return appRunner.Configure(c =>
            {
                c.UseMiddleware(ReplSession, MiddlewareSteps.ReplSession);
                // use the existing appRunner to reuse the configuration.
                c.UseParameterResolver(ctx => new ReplSession(appRunner, replConfig, ctx));
                
                var config = new Config(appRunner, replConfig);
                c.Services.Add(config);

                var replOption = replConfig.ReplOption;
                if (replOption?.IsRequested ?? false)
                {
                    var option = new Option(replOption!.LongName, replOption.ShortName, TypeInfo.Flag, ArgumentArity.Zero)
                    {
                        Description = replOption.Description
                    };
                    config.Option = option;

                    c.BuildEvents.OnCommandCreated += args =>
                    {
                        var builder = args.CommandBuilder;

                        // do not include option if already in a session
                        if (!config.InSession && builder.Command.IsRootCommand())
                        {
                            builder.AddArgument(option);
                        }
                    };
                }
            });
        }

        private class Config
        {
            public AppRunner AppRunner { get; }
            public ReplConfig ReplConfig { get; }
            public bool InSession { get; set; }
            public Option? Option { get; set; }

            public Config(AppRunner appRunner, ReplConfig replConfig)
            {
                AppRunner = appRunner ?? throw new ArgumentNullException(nameof(appRunner));
                ReplConfig = replConfig ?? throw new ArgumentNullException(nameof(replConfig));
            }
        }

        private static Task<int> ReplSession(CommandContext ctx, ExecutionDelegate next)
        {
            var parseResult = ctx.ParseResult!;
            var cmd = parseResult.TargetCommand;
            if (cmd.IsRootCommand() 
                && !cmd.IsExecutable 
                && parseResult.ParseError is null 
                && !parseResult.HelpWasRequested())
            {
                var config = ctx.AppConfig.Services.GetOrThrow<Config>();
                var option = config.Option;
                if (!config.InSession && (option is null || cmd.HasInputValues(option.Name)))
                {
                    config.InSession = true;
                    new ReplSession(config.AppRunner, config.ReplConfig, ctx).Start();
                    return ExitCodes.Success;
                }
            }

            return next(ctx);
        }
    }
}