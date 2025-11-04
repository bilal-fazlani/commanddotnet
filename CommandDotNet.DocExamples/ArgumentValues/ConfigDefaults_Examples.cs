namespace CommandDotNet.DocExamples.ArgumentValues;

public static class ConfigDefaults_Examples
{
    // begin-snippet: config_defaults_model
    public class Creds
    {
        [Option('u', "User")]
        public string? Username { get; set; }

        [EnvVar("ExampleAppAccessToken")]
        [Option('t', "token")]
        public Password? AccessToken { get; set; }
    }
    // end-snippet

    // begin-snippet: config_defaults_envvar
    public class CredsWithEnvVar
    {
        [Option('u', "User")]
        public string? Username { get; set; }

        [EnvVar("ExampleAppAccessToken")]
        [Option('t', "token")]
        public Password? AccessToken { get; set; }
    }
    // end-snippet

    // begin-snippet: config_defaults_appsetting
    public class CredsWithAppSetting
    {
        [Option('u', "User")]
        public string? Username { get; set; }

        [AppSetting("AccessToken")]
        [Option('t', "token")]
        public Password? AccessToken { get; set; }
    }
    // end-snippet
}
