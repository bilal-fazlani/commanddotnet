using CommandDotNet;
using CommandDotNet.Prompts;
using CommandDotNet.Rendering;

namespace CommandDotNet.DocExamples.ArgumentValues;

public static class Prompting_Examples
{
    // begin-snippet: prompting_knock_knock
    public class JokeApp
    {
        [Command(Description = "knock-knock joke, demonstrating use of IPrompter")]
        public void Knock(IConsole console, IPrompter prompter)
        {
            console.Out.WriteLine("Knock knock");
            
            if (prompter.TryPromptForValue("who's there?", out var answer1, out _))
            {
                console.Out.WriteLine($"{answer1} who?");
                
                var answer2 = prompter.PromptForValue("punchline", out bool isCancelled);
                if (!isCancelled)
                {
                    console.Out.WriteLine(answer2);
                }
            }
        }
    }
    // end-snippet

    // begin-snippet: prompting_password_type
    public class LoginApp
    {
        public void Login(string username, Password password)
        {
            // Password characters are hidden during input
            System.Console.WriteLine($"Logging in as {username}");
        }
    }
    // end-snippet
}
