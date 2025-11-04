using CommandDotNet.NameCasing;

namespace CommandDotNet.DocExamples.OtherFeatures;

public static class NameCasing_Examples
{
    // begin-snippet: name_casing_kebab_case
    public class App
    {
        public void MigrateUser([Option]bool dryRun)
        {
            // With kebab-case transformation:
            // command: migrate-user
            // option: --dry-run
        }
    }
    // end-snippet

    // begin-snippet: name_casing_override_attribute
    public class App2
    {
        [Command("migrateUser")]
        public void MigrateUser([Option]bool dryRun)
        {
            // Command name is "migrateUser" (not transformed)
            // Option is still "--dry-run" (transformed)
        }
    }
    // end-snippet

    // begin-snippet: name_casing_usage
    public static int Main(string[] args)
    {
        return new AppRunner<App>()
            .UseNameCasing(Case.KebabCase)
            .Run(args);
    }
    // end-snippet
}
