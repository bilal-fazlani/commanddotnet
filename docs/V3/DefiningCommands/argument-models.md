# Argument Models
Argument models provide a way to bundle a group of arguments in a class.

```c#
public void SendEmail([Option]string subject, [Option(ShortName="a")]List<string> attachments, [Option]string body, [Argument]string from, [Argument]string to)
{

}
```

this can be invoked from the shell with:

```bash
send --subject hi -a "myFile.txt" -a "important.docx" --body "just wanted you to review these files" bilal@bilal.com john@john.com
```

The same arguments can be defined with a class:

```c#
public class Email : IArgumentModel
{
    [Option]
    public string Subject {get;set;}
    
    [Option(ShortName="a")]
    public List<string> Attachments {get;set;}
    
    [Option]
    public string Body {get;set;}
    
    [Operand]
    public string From {get;set;}
    
    [Operand]
    public string To {get;set;}
}
```

and

```c#
public void SendEmail(Email email)
{

}
```

### Composition

An `IArgumentModel` can be composed from other `IArgumentModel`s allowing easy reuse of common arguments.

```c#
public class DryRun : IArgumentModel
{    
    [Option(LongName="dryrun")]
    public bool IsDryRun {get;set;}
}

public class SendEmailArgs : IArgumentModel
{    
    public DryRun DryRun {get;set;}
    
    public Email Email {get;set;}
}
```

### Benefits of argument models:

* Common arguments can be extracted to models to enforce contracts across commands.  <br/>ex. DryRunModel ensures the same short name, long name, description, etc are consistent across all commands using this model.
* [FluentValidation](fluent-validation-for-argument-models.md) framework can be used to validate the model

### Caveat

`Argument` position cannot be guaranteed to be consistent because the .Net Framework does not guarantee the order properties are reflected.

> The [GetProperties](https://docs.microsoft.com/en-us/dotnet/api/system.type.getproperties) method does not return properties in a particular order, such as alphabetical or declaration order. Order can differ on each machine the app is deployed to. Your code must not depend on the order in which properties are returned because that order is no guaranteed.

This is not an issue with `Option` because options are named, not positional

### Recommendation 
* Avoid modelling operands in argument models unless you need to validate them using FluentValidation.
* If you do need to model operands and you have scripts in place, verify the scripts work as expected on each new machine, after .net framework updates and after new deployments.