using System;
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
        
        public T Resolve<T>()
        {
            return _serviceProvider.GetService<T>();
        }

        public object Resolve(Type type)
        {
            return _serviceProvider.GetService(type);
        }
    }
}