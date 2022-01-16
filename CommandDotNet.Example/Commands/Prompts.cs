using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Prompts;
using Spectre.Console;

namespace CommandDotNet.Example.Commands
{
    [Command(
        Description = "Demonstrates prompting (including passwords) and interceptor method to define options common to subcommands",
        ExtendedHelpText = "Uses an interceptor method ",
        Usage = "prompts ")]
    public class Prompts
    {
        [Subcommand]
        public class Secure
        {
            public Task<int> Intercept(InterceptorExecutionDelegate next,
                Password password,
                IConsole console,
                [Option]
                string username = "admin")
            {
                // mimic auth

                if (username == null)
                {
                    console.Out.WriteLine("username not provided");
                    return ExitCodes.Error;
                }

                var pwd = password?.GetPassword();
                if (string.IsNullOrWhiteSpace(pwd))
                {
                    console.Out.WriteLine("password not provided");
                    return ExitCodes.Error;
                }

                console.Out.WriteLine($"authenticated as user:{username} with password:{password}  (actual password:{pwd})");

                return next();
            }

            public void Download(string url, string filepath, IConsole console)
            {
                console.Out.WriteLine($"Pretending to download {url} to {filepath}");
            }
        }

        [Command(Description = "Echos the given text, demonstrating prompting for a single item")]
        public void Echo(IConsole console, [Operand(Description = "the text to echo")]string text)
        {
            console.Out.WriteLine(text);
        }

        [Command(Description = "sums the list of numbers, demonstrating prompting for a list")]
        public void Sum(IConsole console, ICollection<int> numbers)
        {
            console.Out.WriteLine(numbers == null
                ? "no numbers were entered"
                : $"{string.Join(" + ", numbers)} = {numbers.Sum()}");
        }

        [Command(Description = "Echos the list of items")]
        public void List(IConsole console, ICollection<string> items)
        {
            console.Out.WriteLine(string.Join(Environment.NewLine, items));
        }


        [Command(Description = "Confirms required boolean argumentS")]
        public void Confirm(IAnsiConsole console, bool @continue)
        {
            console.WriteLine($"{nameof(@continue)} {@continue}");
        }

        [Command(Description = "knock-knock joke, demonstrating use of IAnsiConsole")]
        public void Choose(IAnsiConsole console, int pageSize = 5)
        {
            var prompt = new SelectionPrompt<string>
            {
                Title = "What is your favorite color?",
                PageSize = pageSize
            }.AddChoices("blue", "purple", "red", "orange", "yellow", "green");
            var answer = console.Prompt(prompt);
            
            console.WriteLine(answer);
        }


        [Command(Description = "knock-knock joke, demonstrating use of IPrompter")]
        public void Knock(IConsole console, IPrompter prompter)
        {
            if (prompter.TryPromptForValue("who's there?", out var answer1, out bool isCancellationRequested) && !isCancellationRequested)
            {
                var answer2 = prompter.PromptForValue($"{answer1} who?", out isCancellationRequested);
        
                console.Out.WriteLine($"{answer1} {answer2}");
                console.Out.WriteLine("lulz");
            }
        }
    }
}