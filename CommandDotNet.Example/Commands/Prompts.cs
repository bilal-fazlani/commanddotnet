using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Rendering;

namespace CommandDotNet.Example.Commands
{
    [Command(
        Description = "Demonstrates prompting (including passwords) and interceptor method to define options common to subcommands",
        ExtendedHelpText = "Uses an interceptor method ",
        Usage = "prompts ")]
    public class Prompts
    {
        public Task<int> Intercept(InterceptorExecutionDelegate next, 
            Password password, 
            IConsole console,
            [Option]
            string username = "admin")
        {
            // mimic auth

            if(username == null)
            {
                console.Out.WriteLine("username not provided");
                return Task.FromResult(1);
            }

            var pwd = password?.GetPassword();
            if (string.IsNullOrWhiteSpace(pwd))
            {
                console.Out.WriteLine("password not provided");
                return Task.FromResult(1);
            }

            console.Out.WriteLine($"authenticated as user:{username} with password:{password}  (actual password:{password.GetPassword()})");

            return next();
        }

        [Command(Description = "Echos the given text, demonstrating prompting for a single item")]
        public void Echo(IConsole console, string text)
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
    }
}