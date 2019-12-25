Right now, when you just execute the dll, without any commands, it shows help. If you want to call a method when application is executed without any 
commands, you can do that with the help of `[DefaultMethod]` attribute.

```c#
[DefaultMethod]
public void SomeMethod()
{
    
}
```

Some points to note about default method:

- It won't show up in help and can't be called explicitly with method name. The only way to execute it is not passing any command name
- It does not support any parameters
- It can have access to class level fields which are passed via constructor