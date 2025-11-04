namespace CommandDotNet.DocExamples.OtherFeatures;

public static class DefaultMiddleware_Examples
{
    public class ValidationApp
    {
        public void Process(string input) { }
    }

    // begin-snippet: default_middleware_basic
    public static int BasicExample(string[] args)
    {
        return new AppRunner<ValidationApp>()
            .UseDefaultMiddleware()
            .Run(args);
    }
    // end-snippet

    // begin-snippet: default_middleware_exclude_debug
    public static int ExcludeDebugDirective(string[] args)
    {
        return new AppRunner<ValidationApp>()
            .UseDefaultMiddleware(excludeDebugDirective: true)
            .Run(args);
    }
    // end-snippet
}
