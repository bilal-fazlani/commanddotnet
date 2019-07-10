using System;
using System.Linq;
using CommandDotNet.Execution;
using CommandDotNet.Help;

namespace CommandDotNet.Builders
{
    internal class HelpOptionSource : IOptionSource
    {
        public void AddOption(ICommandBuilder commandBuilder)
        {
            var option = new Option(Constants.HelpTemplate, ArgumentArity.Zero)
            {
                Description = "Show help information",
                TypeDisplayName = Constants.TypeDisplayNames.Flag,
                IsSystemOption = true,
                Arity = ArgumentArity.Zero
            };

            commandBuilder.AddArgument(option);
        }
        
        internal static int HelpMiddleware(ExecutionContext executionContext, Func<ExecutionContext, int> next)
        {
            if (executionContext.ParseResult.Values.Any(v => v.Argument.Name == Constants.HelpArgumentTemplate.Name))
            {
                Print(executionContext.AppSettings, executionContext.ParseResult.Command);
                return 0;
            }

            return next(executionContext);
        }

        public static void Print(AppSettings appSettings, ICommand command)
        {
            IHelpProvider helpTextProvider = HelpTextProviderFactory.Create(appSettings);
            appSettings.Out.WriteLine(helpTextProvider.GetHelpText(command));
        }
    }
}