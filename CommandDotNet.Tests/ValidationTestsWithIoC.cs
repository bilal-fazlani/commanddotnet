using System;
using CommandDotNet.IoC.MicrosoftDependencyInjection;
using CommandDotNet.Models;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests
{
    public class ValidationTestsWithIoC : TestBase
    {
        public ValidationTestsWithIoC(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void CanValidateModelWithIoC()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<ValidationService>(new ValidationService
            {
                Token = 4
            });

            serviceCollection.AddSingleton<VehicalValidator>();
            
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            
            AppRunner<ValidationAppWithIoC> appRunner = new AppRunner<ValidationAppWithIoC>(new AppSettings
                {
                    Case = Case.KebabCase
                })
                .UseMicrosoftDependencyInjection(serviceProvider);

            appRunner.Run("validate-model").Should().Be(2);
        }
    }

    public class ValidationAppWithIoC
    {
        public void ValidateModel(Vehical vehical)
        {
            Console.WriteLine(vehical.ToJson());
        }
    }

    [Validator(typeof(VehicalValidator))]
    public class Vehical : IArgumentModel
    {
        public int Id { get; set; }
        
        public string RegistrationNumber { get; set; }
        
        public string Model { get; set; }
    }

    public class VehicalValidator : AbstractValidator<Vehical>
    {
        public VehicalValidator(ValidationService validationService)
        {
            if(validationService.Token != 4)
                throw new Exception("Dependency injection failed");
            
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.RegistrationNumber).Length(4);
            RuleFor(x => x.Model).NotEmpty();
        }
    }

    public class ValidationService
    {
        public int Token { get; set; }
    }
}