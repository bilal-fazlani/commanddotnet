using System;

namespace CommandDotNet.Extensions
{
    public static class ServicesExtensions
    {
        public static T GetOrAdd<T>(this IServices services, T key, Func<T> addCallback)
        {
            var service = services.Get<T>();
            
            if (service == null)
            {
                service = addCallback();
                services.Add(service);
            }

            return service;
        }
    }
}