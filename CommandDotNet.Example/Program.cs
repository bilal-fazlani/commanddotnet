using System;
using Autofac;
using CommandDotNet.IoC.Autofac;
using CommandDotNet.IoC.MicrosoftDependencyInjection;
using CommandDotNet.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CommandDotNet.Example
{
    class Program
    {
        static int Main(string[] args)
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IService, Service>();

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            IService service = serviceProvider.GetService<IService>();

            Console.WriteLine(service.value);

            AppRunner<MyApplication> appRunner = new AppRunner<MyApplication>()
                //.UseAutofac(container)
                .UseMicrosoftDependencyInjection(serviceProvider);
            
            return appRunner.Run(args);
        }
    }
}