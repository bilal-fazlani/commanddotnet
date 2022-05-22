using CommandDotNet.LocalizationExample.Commands;
using CommandDotNet.LocalizationExample.Interfaces.Commands;
using CommandDotNet.LocalizationExample.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Globalization;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace CommandDotNet.LocalizationExample
{
    public class Program
    {
        public static ServiceProvider? _serviceProvider;

        public static IConfigurationRoot? configuration;

        private static IStringLocalizer<ErrorMessages>? _ErrorLocalizer;

        static int Main(string[] args)
        {
            //variables
            int exitcode;

            //Creating Service  Collection
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection = ConfigureServices(serviceCollection);

            //Read CulturInfo from Appsettings
            var culture = new CultureInfo(configuration.GetSection("Culture").GetSection("CultureInfo").Value.ToString());

            //Set CultureInfo
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            //Build service provider
            _serviceProvider = serviceCollection.BuildServiceProvider();

            //Set LogLocalizer
            _ErrorLocalizer = GetService<IStringLocalizer<ErrorMessages>>();

            try
            {
                //Run Testapp   
                exitcode = _serviceProvider.GetService<App>().Run(args);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, _ErrorLocalizer.GetString("FatalError"));
                exitcode = 666;
            }
            finally
            {
                Log.CloseAndFlush();
            }

            return exitcode;
        }

        public static T GetService<T>()
        {
            return Program._serviceProvider.GetService<T>();
        }

        public static IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
        {
            //Add Serilog to Servicecollection
            serviceCollection.AddSingleton(LoggerFactory.Create(builder =>
            {
                builder.AddSerilog(
                    Log.Logger = new LoggerConfiguration()
                        .WriteTo.Console(Serilog.Events.LogEventLevel.Error)
                        .MinimumLevel.Debug()
                        .Enrich.FromLogContext()
                        .CreateLogger());
            }));
            serviceCollection.AddLogging();

            //add Localization
            serviceCollection.AddLocalization(options => options.ResourcesPath = "Resources");

            //Create Logger
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(Serilog.Events.LogEventLevel.Debug)
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .CreateLogger();

            //SetUp Configuration Files
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            //SetUp Services
            serviceCollection.AddSingleton(configuration);
            serviceCollection.AddTransient<IMenu, Menu>();
            serviceCollection.AddTransient<Git, Git>();
           

            //Add app
            serviceCollection.AddTransient<App>();
            return serviceCollection;
        }
    }
}