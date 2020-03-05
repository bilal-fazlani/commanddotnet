using System;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using CommandDotNet.Help;
using CommandDotNet.Tokens;

namespace CommandDotNet.Parsing
{
    internal static class TypoSuggestionsMiddleware
    {
        internal static AppRunner UseTypoSuggestions(AppRunner appRunner)
        {
            // -1 to ensure this middleware runs before any prompting so the value won't appear null
            return appRunner.Configure(c => c.UseMiddleware(Middleware,
                MiddlewareSteps.TypoSuggest.Stage, MiddlewareSteps.TypoSuggest.Order));
        }

        private static Task<int> Middleware(CommandContext ctx, ExecutionDelegate next)
        {
            if (ctx.ParseResult.ParseError != null 
                && ctx.ParseResult.ParseError is CommandParsingException cpe 
                && cpe.UnrecognizedArgument != null)
            {
                var @out = ctx.Console.Out;
                var command = cpe.Command;
                var usage = command.GetAppName(ctx.AppConfig.AppSettings.Help) + " " +  command.GetPath();

                switch (cpe.UnrecognizedArgument.TokenType)
                {
                    case TokenType.Option:
                        @out.WriteLine($"{cpe.UnrecognizedArgument.RawValue} is not an option.  See '{usage} --help'");
                        if (command.Options.Any())
                        {
                            @out.WriteLine();
                            @out.WriteLine("The most similar option is");
                            if (cpe.UnrecognizedArgument.OptionTokenType.IsLong)
                            {
                                command.Options.Where(o => !o.LongName.IsNullOrWhitespace()).ForEach(o => @out.WriteLine($"   --{o.LongName}"));
                            }
                            else
                            {
                                command.Options.Where(o => o.ShortName != null).ForEach(o => @out.WriteLine($"   -{o.ShortName}"));
                            }
                        }
                        break;
                    case TokenType.Value:
                        @out.WriteLine($"{cpe.UnrecognizedArgument.RawValue} is not a command or argument.  See '{usage} --help'");
                        if (command.Subcommands.Any())
                        {
                            @out.WriteLine();
                            @out.WriteLine("The most similar command is");
                            command.Subcommands.ForEach(c => @out.WriteLine($"   {c.Name}"));
                        }
                        if (command.Operands.Any())
                        {
                            @out.WriteLine();
                            @out.WriteLine("The most similar argument is");
                            command.Operands.ForEach(c => @out.WriteLine($"   {c.Name}"));
                        }
                        break;
                    case TokenType.Directive:
                        // TODO: suggest other directives? We'd need a list of names which we don't collect atm.
                    default:
                        throw new ArgumentOutOfRangeException($"unknown {nameof(TokenType)}: {cpe.UnrecognizedArgument.TokenType}");
                }
                return Task.FromResult(1);
            }

            return next(ctx);
        }
    }
}