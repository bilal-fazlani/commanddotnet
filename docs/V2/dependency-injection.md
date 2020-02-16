Let's say your app uses a service :

```c#
public interface IService
{
    int GetValue();
}

public class Service : IService
{
    public int GetValue()
    {
        return 4;
    }
}
```

Without dependency injection, app will have to create an instance of `Service` manually:

```c#
public class App
{
    public void PrintServiceValue()
    {
        IService service = new Service();
        Console.WriteLine(service.GetValue().ToString());
    }
}
```

This looks simple now but because the `Service` class constructor doesn't have any constructor parameters but if it had, it would have been difficult for `App` to create the instance of `Service`.

Another reason why you might want to use dependency injection is testable code. This is how `App` looks like with "Property injection"

```c#
public class App
{
    [InjectProperty]
    public IService Service {get;set;}

    public void PrintServiceValue()
    {
        Console.WriteLine(Service.GetValue().ToString());
    }
}
```

The benefit here is that you can mock IService and just test the behavior of `App` without worrying about `Service` class.

CommandDotNet supports two IoC frameworks - Autofac & Microsoft

### Autofac

In order to use autofac, you need to install an additional integration nuget package : https://www.nuget.org/packages/CommandDotNet.IoC.Autofac/

This how you can use the package:

```c#
static int Main(string[] args)
{
    ContainerBuilder containerBuilder = new ContainerBuilder();
    containerBuilder.RegisterType<Service>().As<IService>();
    IContainer container = containerBuilder.Build();
    
    AppRunner<ServiceApp> appRunner = new AppRunner<ServiceApp>().UseAutofac(container);
    
    return appRunner.Run(args);
}
```

### Microsoft

In order to use Microsoft Dependency Injection, you need to install an additional integration nuget package : https://www.nuget.org/packages/CommandDotNet.IoC.MicrosoftDependencyInjection/

This how you can use the package:

```c#
static int Main(string[] args)
{
    IServiceCollection serviceCollection = new ServiceCollection();
    serviceCollection.AddSingleton<IService, Service>();
    IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
    
    AppRunner<ServiceApp> appRunner = new AppRunner<ServiceApp>().UseMicrosoftDependencyInjection(serviceProvider)
    
    return appRunner.Run(args);
}
```