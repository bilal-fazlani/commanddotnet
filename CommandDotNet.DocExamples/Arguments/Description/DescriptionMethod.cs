using System;

namespace CommandDotNet.DocExamples.Arguments.Description;

public class DescriptionMethodExamples
{
    public void Deploy(
        [Option('t', "targets")]
        [DescriptionMethod(nameof(GetTargetsDescription))]
        string[] targets,

        [Option('e', "environment")]
        [DescriptionMethod(nameof(GetEnvironmentsDescription))]
        string environment = "dev")
    {
        Console.WriteLine($"Deploying {string.Join(", ", targets)} to {environment}");
    }

    private static string GetTargetsDescription()
    {
        // Dynamic discovery of available targets
        var availableTargets = new[] { "app", "database", "cache", "notifications" };
        return $"Available targets: {string.Join(", ", availableTargets)}";
    }

    private static string GetEnvironmentsDescription()
    {
        // Could read from config, environment variables, etc.
        var environments = new[] { "dev", "staging", "prod" };
        return $"Available environments: {string.Join(", ", environments)}";
    }
    
}
