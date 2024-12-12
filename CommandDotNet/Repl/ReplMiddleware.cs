using System;
using System.Threading.Tasks;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;

namespace CommandDotNet.Repl
{
    public static class ReplMiddleware
    {
        public static AppRunner UseRepl(this AppRunner appRunner, 
            ReplConfig? replConfig = null, 
            ReplOptionInfo? replOptionInfoForRootCommand = null)
        {
            /*
            * TEST:
            * - ReplSession with parameter resolver
            * - 
            * 
            * - when using option
            *   - show option only
            *     - for root command
            *     - while not in a session
            */

            replConfig ??= new ReplConfig();

            return appRunner.Configure(c =>
            {
                var config = new Config(appRunner, replConfig);
                c.Services.Add(config);

                c.UseParameterResolver(ctx => new ReplSession(appRunner, replConfig.CloneWithPublicProperties(), ctx));

                if (replOptionInfoForRootCommand is not null)
                {
                    c.UseMiddleware(ReplSession, MiddlewareSteps.ReplSession);

                    var option = new Option(replOptionInfoForRootCommand.LongName, replOptionInfoForRootCommand.ShortName, TypeInfo.Flag, ArgumentArity.Zero)
                    {
                        Description = replOptionInfoForRootCommand.Description
                    };
                    config.Option = option;

                    c.BuildEvents.OnCommandCreated += args =>
                    {
                        var builder = args.CommandBuilder;

                        // do not include option if already in a session
                        var command = builder.Command;
                        if (!config.InSession && command.IsRootCommand())
                        {
                            if (command.IsExecutable)
                            {
                                throw new InvalidConfigurationException($"Root command {command.Name} has been defined as executable using [DefaultCommand] on method {command.DefinitionSource}. This is not suitable for hosting an interactive session. " +
                                                                        "Either: A) Do not use this method as the default for the root command " +
                                                                        $"or B) do not define {nameof(replOptionInfoForRootCommand)} and define a " +
                                                                        $"command with a parameter of type {nameof(ReplSession)} to initiate the session.");
                            }
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

            /* Test:
             * - session not entered when
             *   - ParseError
             *   - Help requested
             *   - already in session
             *   - option is { } but not provided
             *   - option is null & cmd is root and not executable
             *
             * - should we check if option is specified but other options also provided?
             *   - No! The app may have provided other options as a config for the session,
             *     available via ReplSession.SessionContext
             */
            var parseResult = ctx.ParseResult!;
            var cmd = parseResult.TargetCommand;

            if (parseResult.ParseError is not null
                || parseResult.HelpWasRequested())
            {
                return next(ctx);
            }

            var config = ctx.AppConfig.Services.GetOrThrow<Config>();
            if (config.InSession)
            {
                return next(ctx);
            }

            var option = config.Option;

            bool ReplSessionWasRequested()
            {
                return cmd.HasInputValues(option.Name);
            }

            if (option is not null && cmd.IsRootCommand() && ReplSessionWasRequested())
            {
                config.InSession = true;
                new ReplSession(config.AppRunner, config.ReplConfig.CloneWithPublicProperties(), ctx).Start();
                return ExitCodes.Success;
            }

            return next(ctx);
        }
    }
}