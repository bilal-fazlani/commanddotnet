namespace CommandDotNet.DocExamples.OtherFeatures;

public static class Suggest_Examples
{
    // begin-snippet: suggest_basic_commands
    public class GitApp
    {
        [Command]
        public void Commit(string message) { }
        
        [Command]
        public void Push() { }
        
        [Command]
        public void Pull() { }
    }
    // end-snippet

    // begin-snippet: suggest_enum_values
    public enum LogLevel { Debug, Info, Warning, Error }

    public class App
    {
        public void SetLogLevel(LogLevel level)
        {
            System.Console.WriteLine($"Log level set to {level}");
        }
    }
    // end-snippet

    // begin-snippet: suggest_nested_subcommands
    public class App2
    {
        [Subcommand]
        public class Remote
        {
            public void Add(string name, string url) { }
            public void Remove(string name) { }
        }
    }
    // end-snippet
}
