```c#
[ApplicationMetadata(Name="send", Description="sends email")]
public void SendEmail([Option]string subject, [Option(ShortName="a")]List<string> attachments, [Option]string body, string from, string to)
{

}
```

this can turns into:

```bash
send --subject hi -a "myFile.txt" -a "important.docx" --body "just wanted you to review these files" bilal@bilal.com john@john.com
```

but the same can be achieved with :

```c#
public class Email : IArgumentModel
{
    [Option]
    public string Subject {get;set;}
    
    [Option(ShortName="a")]
    public List<string> Attachments {get;set;}
    
    [Option]
    public string Body {get;set;}
    
    public string From {get;set;}
    
    public string To {get;set;}
}
```

and

```c#
[ApplicationMetadata(Name="send", Description="sends email")]
public void SendEmail(Email email)
{

}
```