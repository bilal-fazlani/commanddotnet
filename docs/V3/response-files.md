# Response Files

Response files are text files containing arguments that can be replaced by their contents in the command line.
Micrsoft uses them in several applications including [msbuild](https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild-response-files?view=vs-2019) and the [MIDL compiler](https://docs.microsoft.com/en-us/windows/win32/midl/response-files). 

Lets start with an example:

``` c#
public void Interceptor(string user, string pwd, string url)
{
    _client.Connect(user, pwd, url);
}

public void Find(string filter, IConsole console)
{
    _client.Find(filter).ForEach(i => console.Out.WriteLine(i.Id));
}

public void Migrate(List<string> ids)
{
    ids.ForEach(_client.Migrate);
}
```

This app has 2 commands that would be called like this:

 * Find: `--user bob --pwd shhh -url dev.site.com Find isNew`
 * Migrate: `--user bob --pwd shhh -url dev.site.com Migrate 12 3 44`

The credentials could be stored in a file: creds/bobs-creds.rsp
``` ini
# bob's dev creds
--user bob --pwd shhhh
--url dev.site.com
```

Then the call to Find could be `@creds/bobs-creds.rsp Find isNew`.

The `expand-response-files` token transformation will replace `@creds/bobs-creds.rsp` with `--user bob --pwd shhhh --url dev.site.com`.

Rules:

* The file path must be prefixed with `@`
* Empty lines and lines starting with `#` are skipped
* Each line is run through a `CommandLineStringSplitter` to split arguments while honoring quoted strings
* Multiple arguments can be specified on a single line
* Arguments can be specified on multiple lines.
* Files can have any extension as long as the contents are text.

## Recipes

There are a number of ways to use response files to improve usability and testiblity of your app.

### Inter-command communication

Similar to [piped arguments](piped-arguments.md), a command could write arguments to a response file for use in another command.
As an extra step, the path to the response file could be the piped argument.

### Resume Session

A long running application could maintain a session file with the last processed key or next key or a list of remaining keys. 
If the app is cancelled or crashes, the user could start from scratch or select a @session-yyyymmdd.rsp to resume the operation.

### Fail File

An application can store ids of failed items in failed.txt file that can be used to rerun just the failed items.
For example:

``` c#
public void Migrate([Option] string failFilePath, List<string> ids)
{
    if(!ids.Any())
    {
        ids = _repo.GetAllIds();
    }
    foreach(var id in ids)
    {
        try
        {
            _client.Migrate(id);
        }
        catch(Exception ex)
        {
            Log.Error(ex);
            if(failFilePath != null)
            {
                File.AppendAllText(failFilePath, id + Environment.NewLine);
            }
        }
    }
}
```

First run: `Migrate --failFilePath failed1.txt` to migrate all items and capture failed ids to `failed1.txt`

Trouble shoot errors and deploy code with bug fixes.

Second run: `Migrate --failFilePath failed2.txt @failed1.txt` to migrate all items in `failed1.txt` and capture failed ids to `failed2.txt`

Rinse and repeat.

### Testing

Response files can be used like configuration files. You can store them for different testing configurations and share them with your team.
This can be more efficient than creating batch scripts because you can change some options or in some cases, reuse the same response file across different commands. 