Typically when a console app exits with no erros, it returns `0` exit code. If there there was an error, it return `1`. 
But there are many possiblities and developers use this exit code to convey details about what exactly happenned. For example,
https://msdn.microsoft.com/en-us/library/ms681381.aspx 

When you write a command line application you can return a custom return code.

I added a new method in my Calculator to accept a number as input and exit the application with that number as exit code.

```c#
[ApplicationMetadata(Description = "Return with code 5", Name = "return")]
public int ReturnCode()
{
    return 5;
}
```

So now when I call this method from console `dotnet example.dll return`, the command ends with an exit code of 5.

!!! important
    Main method's return type should be int for this to work
