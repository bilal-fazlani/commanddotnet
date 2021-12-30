using System;
using CommandDotNet.Prompts;
using CommandDotNet.TestTools.Prompts;
using NUnit.Framework;

namespace CommandDotNet.DocExamples.Arguments.Arguments
{
    [TestFixture]
    public class Passwords
    {
        public class Program
        {
            public static AppRunner AppRunner => new AppRunner<Program>().UsePrompter();

            // begin-snippet: passwords_login
            public void Login(IConsole console, string username, Password password)
            {
                console.WriteLine($"u:{username} p:{password}");
            }
            // end-snippet

            // begin-snippet: passwords_prompt
            public void Prompt(IConsole console, IPrompter prompter, string username)
            {
                var password = prompter.PromptForValue("password", out _, isPassword: true);
                console.WriteLine($"u:{username} p:{password}");
            }
            // end-snippet
        }

        public static BashSnippet Login = new("passwords_login_exe",
            Program.AppRunner, "myapp.exe", "Login roy rogers", 0,
            @"u:roy p:*****");

        public static BashSnippet Prompt = new("passwords_prompt_exe",
            Program.AppRunner, "myapp.exe", "Prompt roy", 0,
            @"password: 
u:roy p:rogers",
            promptAnswers: new IAnswer[]{ new Answer("rogers") });

        [Test] public void Obligatory_test_since_snippets_cover_all_cases() => Assert.True(true);
    }
}