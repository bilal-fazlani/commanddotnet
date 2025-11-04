# AppSettings and Environment Variables

Populate default values using appSettings and environment variables. 

These defaults will be shown as the default values in help output.

The values are converted as they would be if entered in the shell.

Collections can be delimited with a comma. eg. `apple,banana,orange`

### Example App

We'll use the following app for examples below.
<!-- snippet: config_defaults_model -->
<a id='snippet-config_defaults_model'></a>
```cs
public class Creds
{
    [Option('u', "User")]
    public string? Username { get; set; }

    [EnvVar("ExampleAppAccessToken")]
    [Option('t', "token")]
    public Password? AccessToken { get; set; }
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/ArgumentValues/ConfigDefaults_Examples.cs#L5-L15' title='Snippet source file'>snippet source</a> | <a href='#snippet-config_defaults_model' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

In this app, the upload and download commands can authenticate with username/password or with an access token.

```bash
dotnet example.dll upload -u me -p my_password http://mysite.com c:\myfile.txt
```
or
```bash
dotnet example.dll upload -t my_token http://mysite.com c:\myfile.txt
```


## Environment Variables
Enable the feature with `appRunner.UseDefaultsFromEnvVar()`.

Use the `[EnvVar("key")]` to assign the environment variable as the default for the argument.

With our example, if we wanted to load the access token from an environment variable, decorate the argument as

<!-- snippet: config_defaults_envvar -->
<a id='snippet-config_defaults_envvar'></a>
```cs
public class CredsWithEnvVar
{
    [Option('u', "User")]
    public string? Username { get; set; }

    [EnvVar("ExampleAppAccessToken")]
    [Option('t', "token")]
    public Password? AccessToken { get; set; }
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/ArgumentValues/ConfigDefaults_Examples.cs#L17-L27' title='Snippet source file'>snippet source</a> | <a href='#snippet-config_defaults_envvar' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

When the _ExampleAppAccessToken_ environment variable exists, the default will be that value.

## AppSettings
Enable the feature with `appRunner.UseDefaultsFromAppSetting(appSettings)`

Use the `[AppSetting("key")]` to assign the app setting as the default for the argument.

<!-- snippet: config_defaults_appsetting -->
<a id='snippet-config_defaults_appsetting'></a>
```cs
public class CredsWithAppSetting
{
    [Option('u', "User")]
    public string? Username { get; set; }

    [AppSetting("AccessToken")]
    [Option('t', "token")]
    public Password? AccessToken { get; set; }
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/ArgumentValues/ConfigDefaults_Examples.cs#L29-L39' title='Snippet source file'>snippet source</a> | <a href='#snippet-config_defaults_appsetting' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

When the _ExampleAppAccessToken_ app setting exists, the default will be that value.

### AppSetting By Convention
Enable the feature with `appRunner.UseDefaultsFromAppSetting(appSettings, includeNamingConventions = true)`

`includeNamingConventions` will enable users to provide defaults for all arguments in your app.

Users can set app settings, using argument names and option short or long names as shown in the help as the setting keys. 

```xml
<add key="--user" value="me"/>
<add key="-p" value="my_secret"/>
<add key="url" value="http://mysite.com"/>
```

Any command with these arguments will inherit the default values from the app settings.

To specify defaults for a specific command, prefix the key with the command name.

```xml
<add key="upload url" value="http://my-upload-site.com"/>
<add key="upload --password" value="secret1"/>
<add key="download url" value="http://my-download-site.com"/>
<add key="download -p" value="secret2"/>
```

## .Net Core Config
Enable the feature with `appRunner.UseDefaultsFromConfig(...)` and use the pattern below

```cs
var appConfig = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", true, true)
    .Build();

var evConfig = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .Build();

new AppRunner<App>()
    .UseDefaultsFromConfig(DefaultSources.GetValueFunc("AppSetting", 
        key => appConfig[key],
        DefaultSources.AppSetting.GetKeyFromAttribute
        // includeNamingConventions as described above
        DefaultSources.AppSetting.GetKeysFromConvention))
    .UseDefaultsFromConfig(DefaultSources.GetValueFunc("EnvVar", key => evConfig[key]))
    .Run(args)
```

Creating two different configs allows us to determine the source of the default value 
for the [Parse directive](../Diagnostics/parse-directive.md) and the [CommandLogger](../Diagnostics/command-logger.md)

## Other Configs
Enable the feature with `appRunner.UseDefaultsFromConfig(arg => ...)`

Example: Say we need git .config settings and have a `GitConfigService` to read them...

Given an app like this

```cs
[AttributeUsage(AttributeTargets.Property|AttributeTargets.Parameter)]
public class GitConfigAttribute : Attribute
{
    public string Key {get;}
    public GitConfigAttribute(string key) => Key = key;
}

public class GitConfig : IArgumentModel
{
    [Option]
    [GitConfig("user.name")]
    public string UserName {get;set;}

    [Option]
    [GitConfig("user.option")]
    public string Email {get;set;}
}

public class MyApp
{
    public void FindRepos(GitConfig gitConfig){...}
}
```

Use the defaults from `GitConfigService` like this

```cs
new AppRunner<MyApp>
    .UseDefaultsFromConfig(arg => 
    {
        var key = argument.CustomAttributes
                          .GetCustomAttribute<GitConfigAttribute>()
                          ?.Key
        return key != null && _gitConfigService.TryGetValue(key, out var value)
            ? new ArgumentDefault("git-config", key, value)
            : null;
    }
    .Run(args);
)
```

!!! Note
    If the config value returns null, it is assumed no default value was found for the argument and it is skipped.
    Returning null will __not__ clear already defined default values.
