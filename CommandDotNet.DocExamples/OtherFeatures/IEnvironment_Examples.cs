namespace CommandDotNet.DocExamples.OtherFeatures;

public static class IEnvironment_Examples
{
    // begin-snippet: ienvironment_inject_example
    public class DeployCommand
    {
        public void Deploy(
            IConsole console,
            IEnvironment environment,
            string target)
        {
            console.Out.WriteLine($"Deploying to {target}");
            console.Out.WriteLine($"Current user: {environment.UserName}");
            console.Out.WriteLine($"Machine: {environment.MachineName}");
            
            var apiKey = environment.GetEnvironmentVariable("DEPLOY_API_KEY");
            if (string.IsNullOrEmpty(apiKey))
            {
                console.Error.WriteLine("DEPLOY_API_KEY environment variable not set");
                return;
            }
            
            // Use apiKey for deployment
        }
    }
    // end-snippet
}
