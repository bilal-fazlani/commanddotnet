using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Rendering;

namespace CommandDotNet.Example
{
    [Command(Name = "prompts", Description = "Demonstrates prompting, including password handling.")]
    public class PromptApp
    {
        public Task<int> Intercept(InterceptorExecutionDelegate next, string username, Password password, IConsole console)
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

        [DefaultMethod]
        public void Default(IConsole console, string say)
        {
            console.Out.WriteLine(say);
        }

        public void Sum(IConsole console, ICollection<int> numbers)
        {
            console.Out.WriteLine(numbers == null
                ? "no numbers were entered"
                : $"{string.Join(" + ", numbers)} = {numbers.Sum()}");
        }

        public void List(IConsole console, ICollection<string> items)
        {
            console.Out.WriteLine(string.Join(Environment.NewLine, items));
        }
    }
}