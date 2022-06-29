using System;
using System.Diagnostics.CodeAnalysis;
using CommandDotNet.Builders;
using Microsoft.Extensions.DependencyInjection;

namespace CommandDotNet.IoC.MicrosoftDependencyInjection
{
    public class MicrosoftDependencyInjectionResolver : IDependencyResolver
    {
        private readonly IServiceProvider _serviceProvider;

        public MicrosoftDependencyInjectionResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object Resolve(Type type)
        {
            return _serviceProvider.GetRequiredService(type);
        }

        public bool TryResolve(Type type, [NotNullWhen(true)] out object? item)
        {
            item = _serviceProvider.GetService(type);
            return item is { };
        }
    }
}