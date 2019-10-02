using System;
using CommandDotNet.Builders;

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
            return _serviceProvider.GetService(type);
        }

        public bool TryResolve(Type type, out object item)
        {
            item = Resolve(type);
            return item != null;
        }
    }
}